using Fincore.Application.DTO;
using Fincore.Application.DTO.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces
{
    public interface IGeneralLedgerService
    {
        Task<ApiResponse<List<GeneralLedgerReadDTO>>> GetAllAsync(int page, int pageSize);

        Task<ApiResponse<GeneralLedgerReadDTO>> GetByIdAsync(int id);

        Task<ApiResponse<GeneralLedgerSummaryDTO>> GetSummaryAsync();

        Task<ApiResponse<List<TrialBalanceReadDTO>>> GetTrialBalanceAsync();

        Task<ApiResponse<TrialBalanceSummaryDTO>> GetTrialBalanceSummaryAsync();

        Task<ApiResponse<List<LedgerAccountReadDTO>>> GetLedgerAccountAsync( int accountId, int page,int pageSize);

        Task<ApiResponse<List<AccountingReportReadDTO>>> GetAccountingReportAsync(DateTime? fromDate, DateTime? toDate,int? accountId, int page, int pageSize);
    }
}
