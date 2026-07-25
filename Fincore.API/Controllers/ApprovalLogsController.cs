using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("FixedPolicy")]
    public class ApprovalLogsController : ControllerBase
    {
        private readonly IApprovalLogService _service;

        public ApprovalLogsController(IApprovalLogService service)
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
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Approval Log not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApprovalLogRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = result.ApprovalLogId },
                result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ApprovalLogRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (result == null)
                return NotFound("Approval Log not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Approval Log not found.");

            return Ok("Approval Log deleted successfully.");
        }
    }
}