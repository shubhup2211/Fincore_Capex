using Fincore.Application.DTO;
using Fincore.Application.DTOs.BudgetCategory;

namespace Fincore.Application.Interfaces.BudgetCategory
{
    public interface IBudgetCategoryService
    {
        Task<ApiResponse<string>> AddBudgetCategory(CreateBudgetCategoryDTO dto);

        Task<ApiResponse<List<BudgetCategoryResponseDTO>>> GetBudgetCategories(
            string? categoryName,
            int? departmentId,
            byte? isActive,
            int page,
            int pageSize);

        Task<ApiResponse<BudgetCategoryResponseDTO>> GetBudgetCategoryById(int id);

        Task<ApiResponse<string>> UpdateBudgetCategory(int id, UpdateBudgetCategoryDTO dto);

        Task<ApiResponse<string>> DeleteBudgetCategory(int id);

        Task<ApiResponse<BudgetCategorySummaryDTO>> GetBudgetCategorySummary();

    }
}