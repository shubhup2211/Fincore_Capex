using Fincore.Application.DTO;
using Fincore.Application.DTOs.Budget;

namespace Fincore.Application.Interfaces.Budget
{
    public interface IBudgetService
    {
        Task<ApiResponse<string>> AddBudget(CreateBudgetDTO dto);

        Task<ApiResponse<List<BudgetResponseDTO>>> GetBudgets(
            string? budgetCode,
            string? budgetName,
            string? financialYear,
            int? budgetCategoryId,
            byte? isActive,
            int page,
            int pageSize);

        Task<ApiResponse<BudgetResponseDTO>> GetBudgetById(int id);

        Task<ApiResponse<string>> UpdateBudget(int id, UpdateBudgetDTO dto);

        Task<ApiResponse<string>> DeleteBudget(int id);

        Task<ApiResponse<BudgetSummaryDTO>> GetBudgetSummary();
    }
}