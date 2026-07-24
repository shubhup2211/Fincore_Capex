using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IRFQService
    {
        Task<ApiResponse<string>> CreateRFQ(RFQDTOPost rfq);
        Task<ApiResponse<string>> UpdateRFQ(int id, RFQDTOPost rfq);
        Task<ApiResponse<string>> DeleteRFQ(int id);
        Task<ApiResponse<RFQDTOGet>> GetRFQById(int id);
        Task<ApiResponse<List<RFQDTOGet>>> GetRFQ(int page, int pagesize);
        Task<ApiResponse<string>> SendRFQ(int id);
        Task<ApiResponse<List<QuotationDTOGet>>> GetRFQQuotations(int id);

    }
}
