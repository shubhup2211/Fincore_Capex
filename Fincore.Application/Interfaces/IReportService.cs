using Fincore.Application.DTO;
using Fincore.Application.DTO.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<List<RevenueReportDTO>>> GetRevenueAsync(int page, int pageSize);

        Task<ApiResponse<List<ExpenseReportDTO>>> GetExpenseAsync(int page, int pageSize);

        Task<ApiResponse<List<VendorSpendDTO>>> GetVendorSpendAsync(int page, int pageSize);

        Task<ApiResponse<List<CapexReportDTO>>> GetCapexAsync(int page, int pageSize);

        Task<ApiResponse<List<OpexReportDTO>>> GetOpexAsync(int page, int pageSize);

        Task<ApiResponse<List<BudgetVarianceReportDTO>>> GetBudgetVarianceAsync(int page, int pageSize);

        Task<ApiResponse<ProfitLossReportDTO>> GetProfitLossAsync(int? companyId, int? departmentId,DateTime? fromDate,DateTime? toDate);

        Task<ApiResponse<CashFlowReportDTO>> GetCashFlowAsync(int? companyId, int? departmentId,DateTime? fromDate, DateTime? toDate);

        Task<ApiResponse<List<BalanceSheetReportDTO>>> GetBalanceSheetAsync(int page,int pageSize);
    }

}
