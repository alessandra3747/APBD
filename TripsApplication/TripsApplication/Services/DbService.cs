using TripsApplication.Data;
using TripsApplication.DTOs;
using Microsoft.EntityFrameworkCore;
using TripsApplication.Exceptions;
using TripsApplication.Models;

namespace TripsApplication.Services;


public interface IDbService
{
    Task<TripsResponseDto> GetTripsAsync(int page, int pageSize);
    Task DeleteClientByIdAsync(int clientId);
    Task AssignClientToTripAsync(ClientTripDetailsDto dto);
}


public class DbService(MasterContext data) : IDbService
{
    public async Task<TripsResponseDto> GetTripsAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var tripsQuery = data.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom);

        int totalCount = await tripsQuery.CountAsync();
        
        int allPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await tripsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new TripsResponseDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = trips.Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientNameDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).ToList()
        };

        return result;
    }

    public async Task DeleteClientByIdAsync(int clientId)
    {
        var client = await data.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == clientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with id {clientId} was not found.");
        }

        if (client.ClientTrips.Any())
        {
            throw new InvalidDeletionException($"Client with id {clientId} has assigned trips and cannot be deleted.");
        }
        
        data.Clients.Remove(client);
        
        await data.SaveChangesAsync();
    }

    public async Task AssignClientToTripAsync(ClientTripDetailsDto dto)
    {
        // CHECK IF CLIENT EXISTS
        var existingClient = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
       
        if (existingClient != null)
            throw new ClientExistsException($"Client with pesel {dto.Pesel} already exists.");

        /*  WYMAGANIE 2
         *  Czy klient o takim numerze PESEL jest już zapisany na
            daną wycieczkę - jeśli tak, zwracamy błąd
            -
            Jeśli klient istnieje wyrzucamy błąd, więc teoretycznie nigdy nie dojdziemy poniżej 
            powyższych linii kodu, żeby sprawdzić czy ma przypisaną wycieczkę. 
            Gdybym miała to zaimplementować zrobiłabym to w taki sposób:
            
            bool isAssigned = await data.ClientTrips.AnyAsync(ct => 
                ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip
                );
            if (isAssigned)
            throw new InvalidOperationException($"Client with pesel {dto.Pesel} is already assigned to the trip with id {idTrip}.");
         */
        
        // CHECK IF TRIP EXISTS AND IS IN THE FUTURE
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == dto.IdTrip);
        
        if (trip == null)
            throw new NotFoundException($"Trip with id {dto.IdTrip} not found.");
        
        if (trip.DateFrom <= DateTime.Now)
            throw new InvalidOperationException("Cannot register for a trip that already started.");
        
        
        using var transaction = await data.Database.BeginTransactionAsync();

        try
        {
            var newClient = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };

            data.Clients.Add(newClient);
            await data.SaveChangesAsync();

            var clientTrip = new ClientTrip
            {
                IdClient = newClient.IdClient,
                IdTrip = dto.IdTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = dto.PaymentDate
            };

            data.ClientTrips.Add(clientTrip);
            await data.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
}