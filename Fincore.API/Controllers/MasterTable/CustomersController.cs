using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v1/customers")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomersController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }


        // GET: api/v1/customers?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await customerService
                .GetAllCustomersAsync(pageNumber, pageSize);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // GET: api/v1/customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var response = await customerService
                .GetCustomerByIdAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }


        // POST: api/v1/customers
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerDto createCustomerDto)
        {
            var response = await customerService
                .CreateCustomerAsync(createCustomerDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // PUT: api/v1/customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            var response = await customerService
                .UpdateCustomerAsync(id, updateCustomerDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // DELETE: api/v1/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var response = await customerService
                .DeleteCustomerAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }
    }
}