using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

   
    public class JobSeekerEducationController(IJobSeekersEducationRepository jobSeekersEducationRepository) : ControllerBase
    {
        [HttpGet]
        [Route("/getalljobseekereducation")]
        public async Task<IActionResult> GetAllJobSeekersEducation()
        {
            var jobEduu = await jobSeekersEducationRepository.GetAllJobSeekerEducationAsync();
            return Ok(jobEduu);
        }

        [HttpGet]
        [Route("/getjobseekereducation")]
        public async Task<IActionResult> GetJobSeekerEducationsById(int id)
        {
            var jobSeekerEdu = await jobSeekersEducationRepository.GetJobSeekerEducationByIdAsync(id);
            if (jobSeekerEdu == null)
            {
                return NotFound();
            }
            return Ok(jobSeekerEdu);
        }

        [HttpPost]
        [Route("/submitjobseekereducation")]
        public async Task<IActionResult> SubmitJobSeekerEducation([FromBody] JobSeekerEducation jobSeekerEducationDto)
        {
            if (jobSeekerEducationDto == null)
            {
                return BadRequest();
            }

            var response = await jobSeekersEducationRepository.SubmitJobSeekerEducationAsync(jobSeekerEducationDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deletejobseekereducation")]
        public async Task<IActionResult> DeleteJobSeekerEducation(int id)
        {
            var response = await jobSeekersEducationRepository.DeleteJobSeekerEducationAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}