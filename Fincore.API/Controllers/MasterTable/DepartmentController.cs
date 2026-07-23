using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v1/departments")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }


        // GET ALL
        // GET: api/v1/departments?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await departmentService
                .GetAllDepartmentsAsync(pageNumber, pageSize);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // GET BY ID
        // GET: api/v1/departments/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var response = await departmentService
                .GetDepartmentByIdAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }


        // CREATE
        // POST: api/v1/departments
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(
            [FromBody] DepartmentDTO departmentDTO)
        {
            var response = await departmentService
                .CreateDepartmentAsync(departmentDTO);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // UPDATE
        // PUT: api/v1/departments/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(
            int id,
            [FromBody] DepartmentDTO departmentDTO)
        {
            var response = await departmentService
                .UpdateDepartmentAsync(id, departmentDTO);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // DELETE
        // DELETE: api/v1/departments/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var response = await departmentService
                .DeleteDepartmentAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }
    }
}