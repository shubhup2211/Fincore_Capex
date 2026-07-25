using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {

        private readonly IAuditLogService _auditService;


        public AuditLogsController(
            IAuditLogService auditService)
        {
            _auditService = auditService;
        }



        // GET ALL
        // Example:
        // api/AuditLogs?pageNumber=1&pageSize=10

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {

            var result =
                await _auditService
                .GetAllAsync(pageNumber, pageSize);


            return Ok(result);

        }





        // GET BY ID

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {

            var result =
                await _auditService
                .GetByIdAsync(id);



            if (result == null)
                return NotFound(
                    "Audit log not found."
                );


            return Ok(result);

        }





        // CREATE

        [HttpPost]
        public async Task<IActionResult> Create(
            AuditLogRequestDto dto)
        {

                var result =
                await _auditService
                .CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = result.AuditLogId
                },
                result
            );

        }





        // UPDATE

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(
            long id,
            AuditLogRequestDto dto)
        {


            var result =
                await _auditService
                .UpdateAsync(id, dto);



            if (result == null)
                return NotFound(
                    "Audit log not found."
                );


            return Ok(result);

        }





        // DELETE

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(
            long id)
        {


            var result =
                await _auditService
                .DeleteAsync(id);



            if (!result)
                return NotFound(
                    "Audit log not found."
                );



            return Ok(
                "Audit log deleted successfully."
            );

        }

    }
}