using CountryTripsApplication.Exceptions;
using CountryTripsApplication.Models;
using CountryTripsApplication.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace CountryTripsApplication.Services;

public interface IDbService
{
    public Task<IEnumerable<TripGetDTO>> GetTripsAsync();
    public Task<List<TripGetDTO>> GetClientsTripsByIdAsync(int id);
    public Task<Client> CreateClientAsync(ClientCreateDTO client);
    public Task RegisterClientToTripAsync(int clientId, int tripId);
    public Task DeleteClientFromTripAsync(int clientId, int tripId);
}

public class DbService(IConfiguration config) : IDbService
{
    private readonly string? _connectionString = config.GetConnectionString("Default");

    public async Task<IEnumerable<TripGetDTO>> GetTripsAsync()
    {
        var result = new List<TripGetDTO>();
        
        await using var connection = new SqlConnection(_connectionString);
        
        //SELECT TRIPS AND THEIR INFORMATION WITH THEIR COUNTRIES
        const string sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName" +
                           " FROM Trip t LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip" +
                           " LEFT JOIN Country c ON ct.IdCountry = c.IdCountry";
        
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            result.Add(new TripGetDTO
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                Country = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        
        return result;
    }

    public async Task<List<TripGetDTO>> GetClientsTripsByIdAsync(int id)
    {
        var result = new List<TripGetDTO>();
        
        //SELECT TRIPS AND THEIR INFORMATION WITH THEIR COUNTRIES WHERE CLIENT'S ID IS PROPER
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName" +
                           " FROM Trip t INNER JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip" +
                           " LEFT JOIN Country_Trip cct ON t.IdTrip = cct.IdTrip LEFT JOIN Country c ON cct.IdCountry = c.IdCountry" +
                           " WHERE ct.IdClient = @Id";
        
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new NotFoundException($"Client with id: {id} does not exist");
        }

        while (await reader.ReadAsync())
        {
            result.Add(new TripGetDTO
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                Country = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        //IF TRIPS LIST IS EMPTY
        if (result.Count == 0)
        {
            throw new NotFoundException($"Client with id: {id} does not have any trips");
        }

        return result;
    }

    public async Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        await using var connection = new SqlConnection(_connectionString);
        
        //INSERTING A CLIENT AND GETTING THEIRS ID
        const string sql = "INSERT INTO Clients (FirstName, LastName, Email, Telephone, Pesel)"+
                           " VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); " +
                           "SELECT scope_identity()";
        
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Telephone", client.Telephone);
        command.Parameters.AddWithValue("@Pesel", client.Pesel);
        
        await connection.OpenAsync();
        var id = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Client
        {
            Id = id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Telephone = client.Telephone,
            Pesel = client.Pesel
        };
    }

    public async Task RegisterClientToTripAsync(int clientId, int tripId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        //CHECK IF CLIENT EXISTS
        var clientExistsQuery = "SELECT COUNT(1) FROM Clients WHERE Id = @ClientId";
        var clientExistsCmd = new SqlCommand(clientExistsQuery, connection);
        clientExistsCmd.Parameters.AddWithValue("@ClientId", clientId);
        
        var clientExists = Convert.ToInt32(await clientExistsCmd.ExecuteScalarAsync());
        if (clientExists == 0)
        {
            throw new NotFoundException("Client not found");
        }
        
        //CHECK IF TRIP EXISTS
        var tripExistsQuery = "SELECT COUNT(1) FROM Trips WHERE Id = @TripId";
        var tripExistsCmd = new SqlCommand(tripExistsQuery, connection);
        tripExistsCmd.Parameters.AddWithValue("@TripId", tripId);

        var tripExists = Convert.ToInt32(await tripExistsCmd.ExecuteScalarAsync());
        if (tripExists == 0)
        {
            throw new NotFoundException("Trip not found");
        }
        
        //CHECK FOR CLIENT LIMIT
        var maxPeopleQuery = "SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId";
        var maxPeopleCmd = new SqlCommand(maxPeopleQuery, connection);
        maxPeopleCmd.Parameters.AddWithValue("@TripId", tripId);
        var maxParticipants = Convert.ToInt32(await maxPeopleCmd.ExecuteScalarAsync());
        
        var currentPeopleQuery = "SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @TripId";
        var currentPeopleCmd = new SqlCommand(currentPeopleQuery, connection);
        currentPeopleCmd.Parameters.AddWithValue("@TripId", tripId);
    
        var currentParticipants = Convert.ToInt32(await currentPeopleCmd.ExecuteScalarAsync());

        if (currentParticipants >= maxParticipants)
        {
            throw new ClientLimitExceeded($"Trip with id {tripId} has exceeded max participants");
        }
        
        //BUSINESS LOGIC
        var insertQuery = "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@ClientId, @TripId, @RegisteredAt)";
        var insertCmd = new SqlCommand(insertQuery, connection);
        insertCmd.Parameters.AddWithValue("@ClientId", clientId);
        insertCmd.Parameters.AddWithValue("@TripId", tripId);
        insertCmd.Parameters.AddWithValue("@RegisteredAt", DateTime.UtcNow);

        await insertCmd.ExecuteNonQueryAsync();
        
    }


    public async Task DeleteClientFromTripAsync(int clientId, int tripId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        //CHECK IF REGISTRATION EXISTS
        var registrationExistsQuery = "SELECT COUNT(1) FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId";
        var registrationExistsCmd = new SqlCommand(registrationExistsQuery, connection);
        registrationExistsCmd.Parameters.AddWithValue("@ClientId", clientId);
        registrationExistsCmd.Parameters.AddWithValue("@TripId", tripId);
        var registrationExists = Convert.ToInt32(await registrationExistsCmd.ExecuteScalarAsync());
        if (registrationExists == 0)
        {
            throw new NotFoundException("Registration not found");
        }
        
        //DELETE REGISTRATION
        var deleteQuery = "DELETE FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId";
        var deleteCmd = new SqlCommand(deleteQuery, connection);
        deleteCmd.Parameters.AddWithValue("@ClientId", clientId);
        deleteCmd.Parameters.AddWithValue("@TripId", tripId);

        await deleteCmd.ExecuteNonQueryAsync();
    }
    
}