using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("FixedPolicy")]
    public class NotificationLogsController : ControllerBase
    {
        private readonly INotificationLogService _service;

        public NotificationLogsController(INotificationLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Notification Log not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NotificationLogRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = result.NotificationLogId },
                result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, NotificationLogRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound("Notification Log not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Notification Log not found.");

            return Ok("Notification Log deleted successfully.");
        }
    }
}