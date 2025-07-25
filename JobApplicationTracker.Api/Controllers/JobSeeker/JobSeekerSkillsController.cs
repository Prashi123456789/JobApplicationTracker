using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    
    public class JobSeekerSkillsController(IJobSeekersSkillsRepository jobSeekersSkillsService) : ControllerBase
    {
        [HttpGet]
        [Route("/getalljobseekerskills")]
        public async Task<IActionResult> GetAllJobSeekersSkills()
        {
            var jobSS = await jobSeekersSkillsService.GetAllJobSeekerSkillsAsync();
            return Ok(jobSS);
        }

        [HttpGet]
        [Route("/getjobseekerskills")]
        public async Task<IActionResult> GetJobSeekerSkillsById(int id)
        {
            var jobSeekerSk = await jobSeekersSkillsService.GetJobSeekerSkillsByIdAsync(id);
            if (jobSeekerSk == null)
            {
                return NotFound();
            }
            return Ok(jobSeekerSk);
        }

        [HttpPost]
        [Route("/submitjobseekerskills")]
        public async Task<IActionResult> SubmitJobSeekerSkills([FromBody] JobSeekerSkills jobSeekerSkillsDto)
        {
            if (jobSeekerSkillsDto == null)
            {
                return BadRequest();
            }

            var response = await jobSeekersSkillsService.SubmitJobSeekerSkillsAsync(jobSeekerSkillsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deletejobseekerskills")]
        public async Task<IActionResult> DeleteJobSeekerSkills(int id)
        {
            var response = await jobSeekersSkillsService.DeleteJobSeekerSkillsAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}