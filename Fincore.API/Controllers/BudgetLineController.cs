using Fincore.Application.DTOs.BudgetLine;
using Fincore.Application.Interfaces.BudgetLine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/v1/[action]")]
    [EnableRateLimiting("FixedPolicy")]
    public class BudgetLineController : ControllerBase
    {
        private readonly IBudgetLineService _budgetLineService;

        public BudgetLineController(IBudgetLineService budgetLineService)
        {
            _budgetLineService = budgetLineService;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> AddBudgetLine(CreateBudgetLineDTO dto)
        {
            var response = await _budgetLineService.AddBudgetLine(dto);
            return Ok(response);
        }

        // Get All
        [HttpGet]
        public async Task<IActionResult> GetBudgetLines(
            int? budgetId,
            int? budgetCategoryId,
            byte? isActive,
            int page = 1,
            int pageSize = 5)
        {
            var response = await _budgetLineService.GetBudgetLines(
                budgetId,
                budgetCategoryId,
                isActive,
                page,
                pageSize);

            return Ok(response);
        }

        // Get By Id
        [HttpGet]
        public async Task<IActionResult> GetBudgetLineById(int id)
        {
            var response = await _budgetLineService.GetBudgetLineById(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Update
        [HttpPut]
        public async Task<IActionResult> UpdateBudgetLine(int id, UpdateBudgetLineDTO dto)
        {
            var response = await _budgetLineService.UpdateBudgetLine(id, dto);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Delete
        [HttpDelete]
        public async Task<IActionResult> DeleteBudgetLine(int id)
        {
            var response = await _budgetLineService.DeleteBudgetLine(id);

            if (!response.success)
                return NotFound(response);

            return Ok(response);
        }

        // Summary
        [HttpGet]
        public async Task<IActionResult> GetBudgetLineSummary()
        {
            var response = await _budgetLineService.GetBudgetLineSummary();
            return Ok(response);
        }
    }
}