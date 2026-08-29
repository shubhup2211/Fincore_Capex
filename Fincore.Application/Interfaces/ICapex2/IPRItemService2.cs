using Fincore.Application.DTO;
using Fincore.Application.DTO2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex2
{
    public interface IPRItemService2
    {
        Task<ApiResponse<string>> addPRItem(PRItemDTOPost2 dto);
        Task<ApiResponse<string>> deletePRItem(int prId);
        Task<ApiResponse<List<PRItemDTOGet2>>> getPRItemByPR(int prId);
    }
}
