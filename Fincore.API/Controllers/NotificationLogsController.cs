using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationLogsController : ControllerBase
    {
        private readonly INotificationLogService _service;

        public NotificationLogsController(INotificationLogService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(NotificationLog log)
        {
            return Ok(await _service.CreateAsync(log));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, NotificationLog log)
        {
            var result = await _service.UpdateAsync(id, log);

            if (result == null)
                return NotFound();

            return Ok("Notification log updated successfully.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound("Notification log not found.");

                return Ok("Notification log deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}