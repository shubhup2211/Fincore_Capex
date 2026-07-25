using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("FixedPolicy")]
    public class UserActivityLogsController : ControllerBase
    {
        private readonly IUserActivityLogService _service;

        public UserActivityLogsController(IUserActivityLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("User Activity Log not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserActivityLogRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.UserActivityLogId },
                result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, UserActivityLogRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound("User Activity Log not found.");

            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("User Activity Log not found.");

            return Ok("User Activity Log deleted successfully.");
        }
    }
}