using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    
    public class SkillsController(ISkillsRepository skillService) : ControllerBase
    {
        [HttpGet]
        [Route("/getallskills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await skillService.GetAllSkillsAsync();
            return Ok(skills);
        }

        [HttpGet]
        [Route("/getskillbyid")]
        public async Task<IActionResult> GetSkillsById(int id)
        {
            var skills = await skillService.GetSkillsByIdAsync(id);
            if (skills == null)
            {
                return NotFound();
            }
            return Ok(skills);
        }

        [HttpPost]
        [Route("/submitskills")]
        public async Task<IActionResult> SubmitSkills([FromBody] SkillsDataModel skillsDto)
        {
            if (skillsDto == null)
            {
                return BadRequest();
            }

            var response = await skillService.SubmitSkillsAsync(skillsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deleteskills")]
        public async Task<IActionResult> DeleteSkills(int id)
        {
            var response = await skillService.DeleteSkillsAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}