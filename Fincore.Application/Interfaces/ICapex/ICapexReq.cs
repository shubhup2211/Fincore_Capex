using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface ICapexReq
    {
        Task<ApiResponse<string>> RaiseCapex ( CapexReqDTOPost capex );

        Task<ApiResponse<List<CapexReqDTOGet>>> GetCapex (int page, int pageSize);

        Task<ApiResponse<CapexReqDTOGet>> GetCapexById (int cid);

        Task<ApiResponse<string>> UpdateCapex(int id, CapexReqDTOPost capex);

        Task<ApiResponse<string>> DeleteCapex( int id );

        Task<ApiResponse<string>> SubmitCapex(int id);
        Task<ApiResponse<string>> ApproveCapex(int id);

        Task<ApiResponse<string>> RejectCapex(int id);
    }
}
