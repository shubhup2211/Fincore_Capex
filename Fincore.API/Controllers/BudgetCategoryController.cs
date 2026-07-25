using Fincore.Application.DTOs.BudgetCategory;
using Fincore.Application.Interfaces.BudgetCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/v1/[action]")]
    [EnableRateLimiting("FixedPolicy")]
    public class BudgetCategoryController : ControllerBase
    {
        private readonly IBudgetCategoryService _budgetCategoryService;

        public BudgetCategoryController(IBudgetCategoryService budgetCategoryService)
        {
            _budgetCategoryService = budgetCategoryService;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> AddBudgetCategory(CreateBudgetCategoryDTO dto)
        {
            var response = await _budgetCategoryService.AddBudgetCategory(dto);

            return Ok(response);
        }

        // Get All
        [HttpGet]
        public async Task<IActionResult> GetBudgetCategories(
            string? categoryName,
            int? departmentId,
            byte? isActive,
            int page = 1,
            int pageSize = 5)
        {
            var response = await _budgetCategoryService.GetBudgetCategories(
                categoryName,
                departmentId,
                isActive,
                page,
                pageSize);

            return Ok(response);
        }

        // Get By Id
        [HttpGet]
        public async Task<IActionResult> GetBudgetCategoryById(int id)
        {
            var response = await _budgetCategoryService.GetBudgetCategoryById(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Update
        [HttpPut]
        public async Task<IActionResult> UpdateBudgetCategory(
            int id,
            UpdateBudgetCategoryDTO dto)
        {
            var response = await _budgetCategoryService.UpdateBudgetCategory(id, dto);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Delete
        [HttpDelete]
        public async Task<IActionResult> DeleteBudgetCategory(int id)
        {
            var response = await _budgetCategoryService.DeleteBudgetCategory(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Summary
        [HttpGet]
        public async Task<IActionResult> GetBudgetCategorySummary()
        {
            var response = await _budgetCategoryService.GetBudgetCategorySummary();

            return Ok(response);
        }
    }
}