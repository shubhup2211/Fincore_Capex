using Fincore.Application.DTO;
using Fincore.Application.DTO.Dashboard;

namespace Fincore.Application.Interfaces.Dashboard
{
    public interface IExecutiveService
    {
        Task<ApiResponse<ExecutiveDashboardDto>> GetExecutiveDashboard();
    }
}