using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IPRItemService
    {
        Task<ApiResponse<string>> CreatePRItem(PRItemDTOPost pri);
        Task<ApiResponse<string>> UpdatePRItem(int id, PRItemDTOPost pri);
        Task<ApiResponse<string>> DeletePRItem(int id);
        Task<ApiResponse<PRItemDTOGet>> GetPRItemById(int id);
        Task<ApiResponse<List<PRItemDTOGet>>> GetPRItem(int page, int pagesize);
    }
}
