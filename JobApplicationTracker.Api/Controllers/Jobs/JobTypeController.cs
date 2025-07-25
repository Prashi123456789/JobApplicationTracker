
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    
    public class JobTypeController(IJobTypeRepository jobTypeService) : ControllerBase
    {
        [HttpGet]
        [Route("/getalljobtypes")]
        public async Task<IActionResult> GetAllJobTypes()
        {
            var jobTypes = await jobTypeService.GetAllJobTypesAsync();
            return Ok(jobTypes);
        }

        [HttpGet]
        [Route("/getjobtypebyid")]
        public async Task<IActionResult> GetJobTypeById(int id)
        {
            var jobType = await jobTypeService.GetJobTypeByIdAsync(id);
            if (jobType == null)
            {
                return NotFound();
            }
            return Ok(jobType);
        }

        [HttpPost]
        [Route("/submitjobtype")]
        public async Task<IActionResult> SubmitJobType([FromBody] JobTypesDataModel jobTypeDto)
        {
            if (jobTypeDto == null)
            {
                return BadRequest();
            }

            var response = await jobTypeService.SubmitJobTypeAsync(jobTypeDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deletejobtype")]
        public async Task<IActionResult> DeleteJobType(int id)
        {
            var response = await jobTypeService.DeleteJobTypeAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}