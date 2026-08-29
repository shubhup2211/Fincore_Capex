using Fincore.Application.DTO;
using Fincore.Application.DTO2;
using Fincore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex2
{
    public interface IPRService2
    {
        Task<ApiResponse<string>> raisePR(PRDTOPost2 dto);
        Task<ApiResponse<string>> approvePR(int prId);
        Task<ApiResponse<string>> rejectPR(int prId);
        Task<ApiResponse<string>> submitPR(int prId);
        Task<ApiResponse<PRDTOGet2>> getPRByUser(int page,int pageSize);
        Task<ApiResponse<List<PRDTOGet2>>> getAllPR(int page, int pageSize, IsActive? status, string? search);
        Task<ApiResponse<List<PRDTOGet2>>> getPendingPR(int page, int pageSize, string? search);

    }
}
