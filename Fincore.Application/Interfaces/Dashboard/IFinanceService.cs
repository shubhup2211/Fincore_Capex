using Fincore.Application.DTO;
using Fincore.Application.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.Dashboard
{
    public interface IFinanceService
    {
        Task<ApiResponse<FinanceDashboardDto>> GetFinanceDashboard();
    }
}
