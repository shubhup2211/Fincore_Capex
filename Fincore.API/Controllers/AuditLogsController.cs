using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditService;

        public AuditLogsController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }


        // GET: api/AuditLogs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _auditService.GetAllAsync();

            return Ok(result);
        }


        // GET: api/AuditLogs/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _auditService.GetByIdAsync(id);

            if (result == null)
                return NotFound("Audit log not found.");

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuditLog auditLog)
        {
            ModelState.Remove(nameof(AuditLog.AuditByUser));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auditService.CreateAsync(auditLog);

            return Ok(result);
        }


        // PUT: api/AuditLogs/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AuditLog auditLog)
        {
            ModelState.Remove(nameof(AuditLog.AuditByUser));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auditService.UpdateAsync(id, auditLog);

            if (result == null)
                return NotFound("Audit log not found.");

            return Ok("Audit log updated successfully.");
        }


        // DELETE: api/AuditLogs/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var result = await _auditService.DeleteAsync(id);

                if (!result)
                    return NotFound("Audit log not found.");

                return Ok("Audit log deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}