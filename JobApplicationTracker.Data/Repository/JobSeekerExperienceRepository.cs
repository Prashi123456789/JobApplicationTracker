using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;


public class JobSeekerExperienceRepository : IJobSeekerExperienceRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public JobSeekerExperienceRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<JobSeekerExperience>> GetAllJobSeekerExperienceAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
          SELECT ExperienceId, 
                 JobSeekerId,
                 CompanyName,
                 Position,
                 StartDate,
                 EndDate,
                 Description
          FROM JobSeekerExperience
          """;

        return await connection.QueryAsync<JobSeekerExperience>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekerExperience> GetJobSeekerExperienceByIdAsync(int jobSeekerExperienceId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch job seeker experience by ID
        var sql = """
      SELECT ExperienceId, 
             JobSeekerId,
             CompanyName,
             Position,
             StartDate,
             EndDate,
             Description
      FROM JobSeekerExperience
      WHERE ExperienceId = @ExperienceId
      """;

        var parameters = new DynamicParameters();
        parameters.Add("@ExperienceId", jobSeekerExperienceId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekerExperience>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitJobSeekerExperienceAsync(JobSeekerExperience jobSeekerExperienceDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobSeekerExperienceDto.ExperienceId <= 0)
        {
            // Insert new job seeker experience
            sql = """
        INSERT INTO JobSeekerExperience (JobSeekerId, CompanyName, Position, StartDate, EndDate, Description)
        VALUES (@JobSeekerId, @CompanyName, @Position, @StartDate, @EndDate, @Description);
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing job seeker experience
            sql = """
        UPDATE JobSeekerExperience
        SET 
            CompanyName = @CompanyName,
            Position = @Position,
            StartDate = @StartDate,
            EndDate = @EndDate,
            Description = @Description
        WHERE ExperienceId = @ExperienceId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@ExperienceId", jobSeekerExperienceDto.ExperienceId, DbType.Int32);
        parameters.Add("@JobSeekerId", jobSeekerExperienceDto.JobSeekerId, DbType.Int32);
        parameters.Add("@CompanyName", jobSeekerExperienceDto.CompanyName, DbType.String);
        parameters.Add("@Position", jobSeekerExperienceDto.Position, DbType.String);
        parameters.Add("@StartDate", jobSeekerExperienceDto.StartDate, DbType.DateTime);
        parameters.Add("@EndDate", jobSeekerExperienceDto.EndDate, DbType.DateTime);
        parameters.Add("@Description", jobSeekerExperienceDto.Description, DbType.String);

        var affectedRows = 0;

        if (jobSeekerExperienceDto.ExperienceId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobSeekerExperienceDto.ExperienceId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs seeker experience submitted successfully." : "Failed to submit job seeker experience."
        };
    }
    public async Task<ResponseDto> DeleteJobSeekerExperienceAsync(int jobSeekerExperienceId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job seeker experience by ID
        var sql = """DELETE FROM JobSeekerExperience WHERE ExperienceId = @ExperienceId""";

        var parameters = new DynamicParameters();
        parameters.Add("@ExperienceId", jobSeekerExperienceId, DbType.Int32); // Correct parameter name

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job seeker experience."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Jobs seeker experience deleted successfully."
        };
    }
}