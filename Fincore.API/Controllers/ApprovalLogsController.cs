using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalLogsController : ControllerBase
    {
        private readonly IApprovalLogService _service;

        public ApprovalLogsController(IApprovalLogService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ApprovalLog log)
        {
            return Ok(await _service.CreateAsync(log));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ApprovalLog log)
        {
            var result = await _service.UpdateAsync(id, log);

            if (result == null)
                return NotFound();

            return Ok("Approval log updated successfully.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Approval log deleted successfully.");
        }
    }
}