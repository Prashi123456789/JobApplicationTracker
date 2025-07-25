using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

   
    public class JobsController(IJobsRepository jobsService) : ControllerBase
    {
        [HttpGet]
        [Route("/getalljobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await jobsService.GetAllJobsAsync();
            return Ok(jobs);
        }

        [HttpGet]
        [Route("/getjobsbyid")]
        public async Task<IActionResult> GetJobsById(int id)
        {
            var jobs = await jobsService.GetJobsByIdAsync(id);
            if (jobs == null)
            {
                return NotFound();
            }
            return Ok(jobs);
        }

        [HttpPost]
        [Route("/submitjobs")]
        public async Task<IActionResult> SubmitJobs([FromBody] JobsDataModel jobsDto)
        {
            if (jobsDto == null)
            {
                return BadRequest();
            }

            var response = await jobsService.SubmitJobAsync(jobsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deletejobs")]
        public async Task<IActionResult> DeleteJobs(int id)
        {
            var response = await jobsService.DeleteJobAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}