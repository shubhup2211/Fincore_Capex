using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IVendorSelectionService
    {
        Task<ApiResponse<string>> CreateVendorSelection(VendorSelectionDTOPost vendorSelection);
        Task<ApiResponse<string>> UpdateVendorSelection(int id, VendorSelectionDTOPost vendorSelection);
        Task<ApiResponse<string>> DeleteVendorSelection(int id);
        Task<ApiResponse<VendorSelectionDTOGet>> GetVendorSelectionById(int id);
        Task<ApiResponse<List<VendorSelectionDTOGet>>> GetVendorSelection(int page, int pagesize);
        Task<ApiResponse<List<QuotationComparisonDTO>>> CompareQuotations(int rfqId);

        Task<ApiResponse<string>> SelectVendor(int rfqId, int vendorId);

        Task<ApiResponse<VendorSelectionDTOGet>> GetSelectedVendorByRFQ(int rfqId);
    }
}