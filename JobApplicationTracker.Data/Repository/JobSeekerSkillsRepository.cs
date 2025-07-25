using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class JobSeekerSkillsRepository : IJobSeekersSkillsRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public JobSeekerSkillsRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<JobSeekerSkills>> GetAllJobSeekerSkillsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT JobSeekerSkillsId, 
                         JobSeekerId
                         SkillId,
                         ProficiencyLevel
                         

                  FROM JobSeekerSkills
                  """;


        return await connection.QueryAsync<JobSeekerSkills>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekerSkills> GetJobSeekerSkillsByIdAsync(int jobSeekerSkillsId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to fetch a job Seeker by ID
        var sql = """
                  SELECT JobSeekerId, 
                         SkillId,
                         ProficiencyLevel
                         
                  
                  FROM JobSeekerSkills
                  WHERE JobSeekerId = JobSeekerId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillsId", jobSeekerSkillsId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekerSkills>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitJobSeekerSkillsAsync(JobSeekerSkills jobSeekerSkillsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobSeekerSkillsDto.JobSeekerSkillsId <= 0)
        {
            // Insert new job seeker skill (assumes JobSeekerSkillsId is auto-incremented)
            sql = """
        INSERT INTO JobSeeker_Skills (JobSeekerId, SkillId, ProficiencyLevel)
        VALUES (@JobSeekerId, @SkillId, @ProficiencyLevel);
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing job seeker skill
            sql = """
        UPDATE JobSeeker_Skills
        SET 
            JobSeekerId = @JobSeekerId,
            SkillId = @SkillId,
            ProficiencyLevel = @ProficiencyLevel
        WHERE JobSeekerSkillsId = @JobSeekerSkillsId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillsId", jobSeekerSkillsDto.JobSeekerSkillsId, DbType.Int32);
        parameters.Add("@JobSeekerId", jobSeekerSkillsDto.JobSeekerId, DbType.Int32);
        parameters.Add("@SkillId", jobSeekerSkillsDto.SkillId, DbType.Int32);
        parameters.Add("@ProficiencyLevel", jobSeekerSkillsDto.ProficiencyLevel, DbType.String);

        var affectedRows = 0;

        if (jobSeekerSkillsDto.JobSeekerSkillsId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobSeekerSkillsDto.JobSeekerSkillsId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs seeker skill submitted successfully." : "Failed to submit job seeker skill."
        };
    }
    public async Task<ResponseDto> DeleteJobSeekerSkillsAsync(int jobSeekerSkillsId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job seeker skill by ID
        var sql = """DELETE FROM JobSeeker_Skills WHERE JobSeekerSkillsId = @JobSeekerSkillsId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillsId", jobSeekerSkillsId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job seeker skill."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Jobs seeker skill deleted successfully."
        };
    }
}