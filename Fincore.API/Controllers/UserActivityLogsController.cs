using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityLogsController : ControllerBase
    {
        private readonly IUserActivityLogService _service;

        public UserActivityLogsController(IUserActivityLogService service)
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
        public async Task<IActionResult> Create(UserActivityLog log)
        {
            return Ok(await _service.CreateAsync(log));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UserActivityLog log)
        {
            var result = await _service.UpdateAsync(id, log);

            if (result == null)
                return NotFound();

            return Ok("User activity log updated successfully.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("User activity log deleted successfully.");
        }
    }
}