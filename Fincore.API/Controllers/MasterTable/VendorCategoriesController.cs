using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v1/vendor-categories")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class VendorCategoriesController : ControllerBase
    {
        private readonly IVendorCategoryService vendorCategoryService;

        public VendorCategoriesController(
            IVendorCategoryService vendorCategoryService)
        {
            this.vendorCategoryService = vendorCategoryService;
        }


        // GET: api/v1/vendor-categories?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllVendorCategories(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var response = await vendorCategoryService
                .GetAllVendorCategoriesAsync(pageNumber, pageSize);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // GET: api/v1/vendor-categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorCategoryById(
            int id)
        {
            var response = await vendorCategoryService
                .GetVendorCategoryByIdAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }


        // POST: api/v1/vendor-categories
        [HttpPost]
        public async Task<IActionResult> CreateVendorCategory(
            [FromBody] CreateVendorCategoryDto createVendorCategoryDto)
        {
            var response = await vendorCategoryService
                .CreateVendorCategoryAsync(createVendorCategoryDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // PUT: api/v1/vendor-categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorCategory(
            int id,
            [FromBody] UpdateVendorCategoryDto updateVendorCategoryDto)
        {
            var response = await vendorCategoryService
                .UpdateVendorCategoryAsync(
                    id,
                    updateVendorCategoryDto);

            if (!response.success)
                return BadRequest(response);

            return Ok(response);
        }


        // DELETE: api/v1/vendor-categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorCategory(
            int id)
        {
            var response = await vendorCategoryService
                .DeleteVendorCategoryAsync(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }
    }
}