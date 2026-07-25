using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IPRService
    {
        Task<ApiResponse<string>> CreatePR(PRDTOPost pr);
        Task<ApiResponse<string>> UpdatePR(int id, PRDTOPost pr);
        Task<ApiResponse<string>> DeletePR(int id);
        Task<ApiResponse<PRDTOGet>> GetPRById(int id);
        Task<ApiResponse<List<PRDTOGet>>> GetPR(int page,int pagesize);

        Task<ApiResponse<string>> SubmitPR(int id);
        Task<ApiResponse<string>> ApprovePR(int id);
        Task<ApiResponse<string>> RejectPR(int id);

    }
}
