using Fincore.Application.DTO;
using Fincore.Application.DTO2;
using Fincore.Application.DTOs.BudgetLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex2
{
    public interface ICapexService
    {
        Task<ApiResponse<string>> RaiseCapex(CapexDTOPost dto);
        Task<ApiResponse<string>> ApproveCapex(int id);
        Task<ApiResponse<string>> RejectCapex(int id);
        Task<ApiResponse<List<CapexDTOGet>>> GetCapexByUserId(int page, int pageSize);
        Task<ApiResponse<List<CapexDTOGet>>> GetAllCapex(int page,int pageSize);
        Task<ApiResponse<List<CapexDTOGet>>> GetPendingCapex(int page, int pageSize);
        Task<ApiResponse<List<BudgetLineResponseDTO>>> GetBudgetLinesByDepartment();


    }
}
