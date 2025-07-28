using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsRepository _jobsService;

        public JobsController(IJobsRepository jobsService)
        {
            _jobsService = jobsService;
        }

        [HttpGet("getalljobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobsService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("getjobsbyid/{id}")]
        public async Task<IActionResult> GetJobsById(int id)
        {
            var job = await _jobsService.GetJobsByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost("submitjobs")]
        public async Task<IActionResult> SubmitJobs([FromBody] JobsDataModel jobsDto)
        {
            if (jobsDto == null)
            {
                return BadRequest("Job data is required.");
            }

            var response = await _jobsService.SubmitJobAsync(jobsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("deletejobs/{id}")]
        public async Task<IActionResult> DeleteJobs(int id)
        {
            var response = await _jobsService.DeleteJobAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}