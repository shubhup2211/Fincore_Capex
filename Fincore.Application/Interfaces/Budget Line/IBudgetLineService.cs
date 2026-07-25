using Fincore.Application.DTO;
using Fincore.Application.DTOs.BudgetLine;

namespace Fincore.Application.Interfaces.BudgetLine
{
    public interface IBudgetLineService
    {
        Task<ApiResponse<string>> AddBudgetLine(CreateBudgetLineDTO dto);

        Task<ApiResponse<List<BudgetLineResponseDTO>>> GetBudgetLines(
            int? budgetId,
            int? budgetCategoryId,
            byte? isActive,
            int page,
            int pageSize);

        Task<ApiResponse<BudgetLineResponseDTO>> GetBudgetLineById(int id);

        Task<ApiResponse<string>> UpdateBudgetLine(int id, UpdateBudgetLineDTO dto);

        Task<ApiResponse<string>> DeleteBudgetLine(int id);

        Task<ApiResponse<BudgetLineSummaryDTO>> GetBudgetLineSummary();
    }
}