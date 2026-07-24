using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IQuotationService
    {
        Task<ApiResponse<string>> CreateQuotation(QuotationDTOPost quotation);
        Task<ApiResponse<string>> UpdateQuotation(int id, QuotationDTOPost quotation);
        Task<ApiResponse<string>> DeleteQuotation(int id);
        Task<ApiResponse<QuotationDTOGet>> GetQuotationById(int id);
        Task<ApiResponse<List<QuotationDTOGet>>> GetQuotation(int page, int pagesize);
        Task<ApiResponse<string>> SubmitQuotation(int id);
        Task<ApiResponse<List<QuotationDTOGet>>> GetVendorQuotations(int vendorId);
        Task<ApiResponse<List<QuotationDTOGet>>> GetRFQQuotations(int rfqId);
    }
}