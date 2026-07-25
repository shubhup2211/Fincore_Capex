using Fincore.Application.DTO;
using Fincore.Application.DTO.Dashboard;
using Fincore.Application.Interfaces.Dashboard;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.Dashboard
{
    public class DashboardBudgetService : IDashboardBudgetService
    {
         AppDbContext db;

        public DashboardBudgetService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<ApiResponse<BudgetDashboardDto>> GetBudgetDashboard()
        {
            var totalBudget = await db.Budgets
                .SumAsync(x => x.BudgetAmount);

            var usedBudget = await db.Budgets
                .Where(x => x.IsActive == 1)
                .SumAsync(x => x.BudgetAmount);

            var dashboard = new BudgetDashboardDto
            {
                TotalBudget = totalBudget,
                UsedBudget = usedBudget,
                RemainingBudget = totalBudget - usedBudget,

                TotalBudgets = await db.Budgets.CountAsync(),

                ActiveBudgets = await db.Budgets
                    .Where(x => x.IsActive == 1)
                    .CountAsync(),

                InactiveBudgets = await db.Budgets
                    .Where(x => x.IsActive == 0)
                    .CountAsync(),

                CurrentFinancialYear = await db.Budgets
                    .OrderByDescending(x => x.FinancialYear)
                    .Select(x => x.FinancialYear)
                    .FirstOrDefaultAsync()
            };

            if (dashboard.TotalBudget > 0)
            {
                dashboard.BudgetUtilizationPercentage =
                    (dashboard.UsedBudget * 100) / dashboard.TotalBudget;
            }

            return ApiResponseHelper.SuccessRes(
                dashboard,
                "Budget Dashboard fetched successfully");
        }
    }
}