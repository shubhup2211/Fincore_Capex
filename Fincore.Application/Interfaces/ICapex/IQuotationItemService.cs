using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IQuotationItemService
    {
        Task<ApiResponse<string>> CreateQuotationItem(QuotationItemDTOPost quotationItem);
        Task<ApiResponse<string>> UpdateQuotationItem(int id, QuotationItemDTOPost quotationItem);
        Task<ApiResponse<string>> DeleteQuotationItem(int id);
        Task<ApiResponse<QuotationItemDTOGet>> GetQuotationItemById(int id);
        Task<ApiResponse<List<QuotationItemDTOGet>>> GetQuotationItem(int page, int pagesize);
    }
}