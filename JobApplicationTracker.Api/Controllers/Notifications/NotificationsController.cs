using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Api.Controllers.JobApplication

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

   
    public class NotificationsController(INotificationsRepository notificationsService) : ControllerBase
    {
        [HttpGet]
        [Route("/getallnotifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await notificationsService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet]
        [Route("/getnotificationbyid")]
        public async Task<IActionResult> GetNotificationsById(int id)
        {
            var notification = await notificationsService.GetNotificationsByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        [HttpPost]
        [Route("/submitnotifications")]
        public async Task<IActionResult> SubmitNotifications([FromBody] NotificationsDataModel notificationsDto)
        {
            if (notificationsDto == null)
            {
                return BadRequest();
            }

            var response = await notificationsService.SubmitNotificationsAsync(notificationsDto);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Route("/deletenotifications")]
        public async Task<IActionResult> DeleteNotifications(int id)
        {
            var response = await notificationsService.DeleteNotificationsAsync(id);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}