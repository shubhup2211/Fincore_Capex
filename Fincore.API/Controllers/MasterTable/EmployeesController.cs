using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v1/employees")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }


        // GET: api/v1/employees?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await employeeService
                .GetAllEmployeesAsync(pageNumber, pageSize);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // GET: api/v1/employees/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var response = await employeeService
                .GetEmployeeByIdAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }


        // POST: api/v1/employees
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(
            [FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var response = await employeeService
                .CreateEmployeeAsync(createEmployeeDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // PUT: api/v1/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            var response = await employeeService
                .UpdateEmployeeAsync(id, updateEmployeeDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // DELETE: api/v1/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var response = await employeeService
                .DeleteEmployeeAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }
    }
}