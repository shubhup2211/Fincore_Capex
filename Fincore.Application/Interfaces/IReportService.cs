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
        Task<List<RevenueReportDTO>> GetRevenueAsync();

        Task<List<ExpenseReportDTO>> GetExpenseAsync();

        Task<List<VendorSpendDTO>> GetVendorSpendAsync();

        Task<List<CapexReportDTO>> GetCapexAsync();

        Task<List<OpexReportDTO>> GetOpexAsync();

        Task<List<BudgetVarianceReportDTO>> GetBudgetVarianceAsync();

        Task<ProfitLossReportDTO> GetProfitLossAsync();

        Task<CashFlowReportDTO> GetCashFlowAsync();
    }

}
