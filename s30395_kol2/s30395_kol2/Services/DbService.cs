using System.Data;
using Microsoft.Data.SqlClient;
using s30395_kol2.DTOs;
using s30395_kol2.Exceptions;

namespace s30395_kol2.Services;

public interface IDbService
{
    public Task<IEnumerable<StudentDetailsGetDto>> GetStudentDetailsAsync();
    public Task<StudentDetailsGetDto> CreateStudentAsync(StudentCreateDto studentData);
}

public class DbService(IConfiguration conf) : IDbService
{
    private async Task<SqlConnection> GetConnectionAsync()
    {
        var connection = new SqlConnection(conf.GetConnectionString("Default-db"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        return connection;
    }
    
    
    public async Task<IEnumerable<StudentDetailsGetDto>> GetStudentDetailsAsync()
    {
        
        var studentsDict = new Dictionary<int, StudentDetailsGetDto>();    
        
        await using var connection = await GetConnectionAsync();
        
        var sql = """
                  SELECT s.Id, s.FirstName, s.LastName, s.Email 
                  FROM Student S
                  LEFT JOIN GroupAssignment GA ON S.Id = GA.Student_Id
                  LEFT JOIN Group G ON GA.Group_Id = G.Id
                  """;
        
        await using var command = new SqlCommand(sql, connection);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var studentId = reader.GetInt32(0);

            if (!studentsDict.TryGetValue(studentId, out var studentDetails))
            {
                studentDetails = new StudentDetailsGetDto
                {
                    Id = studentId,
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    Groups = []
                };
                
                studentsDict.Add(studentId, studentDetails);
            }

            if (!await reader.IsDBNullAsync(4))
            {
                studentDetails.Groups.Add(new StudentGroupGetDto
                {
                    Id = reader.GetInt32(4),
                    Name = reader.GetString(5),
                    AcademicYear = reader.GetDateTime(6),
                    Level = reader.GetString(7),
                });
            }
            
        }
        
        return studentsDict.Values;
    }

    
    public async Task<StudentDetailsGetDto> CreateStudentAsync(StudentCreateDto studentData)
    {
        await using var connection = await GetConnectionAsync();
        
        var groups = new List<StudentGroupGetDto>();

        if (studentData.GroupAssignments is not null && studentData.GroupAssignments.Count != 0)
        {
            foreach (var group in studentData.GroupAssignments)
            {
                var groupCheckSql = """
                                    SELECT Id, Name, Level, AcademicYear
                                    FROM Group
                                    WHERE Id = @Id;
                                    """;
                
                await using var command = new SqlCommand(groupCheckSql, connection);
                command.Parameters.AddWithValue("@Id", group);
                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    throw new NotFoundException($"Group with id {group} does not exist");
                }
                
                groups.Add(new StudentGroupGetDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Level = reader.GetString(2),
                    AcademicYear = reader.GetDateTime(2)
                });
            }
        }

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var createStudentSql = """
                                   INSERT INTO Student
                                   OUTPUT inserted.Id
                                   VALUES (@FirstName, @LastName, @Email);
                                   """;

            await using var createStudentCommand =
                new SqlCommand(createStudentSql, connection, (SqlTransaction)transaction);
            
            createStudentCommand.Parameters.AddWithValue("@FirstName", studentData.FirstName);
            createStudentCommand.Parameters.AddWithValue("@LastName", studentData.LastName);
            createStudentCommand.Parameters.AddWithValue("@Email", studentData.Email);

            
            var createdStudentId = Convert.ToInt32(await createStudentCommand.ExecuteScalarAsync());
            
            foreach (var group in groups)
            {
                var groupAssignmentSql = """
                                         INSERT INTO GroupAssignment 
                                         values (@StudentId, @GroupId);
                                         """;
                await using var groupAssignmentCommand =
                    new SqlCommand(groupAssignmentSql, connection, (SqlTransaction)transaction);
                groupAssignmentCommand.Parameters.AddWithValue("@StudentId", createdStudentId);
                groupAssignmentCommand.Parameters.AddWithValue("@GroupId", group.Id);

                await groupAssignmentCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();

            return new StudentDetailsGetDto
            {
                Id = createdStudentId,
                FirstName = studentData.FirstName,
                LastName = studentData.LastName,
                Email = studentData.Email,
                Groups = groups
            };

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
}