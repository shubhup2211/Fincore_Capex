using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IRFQVendorService
    {
        Task<ApiResponse<string>> CreateRFQVendor(RFQVendorDTOPost rfqVendor);
        Task<ApiResponse<string>> UpdateRFQVendor(int id, RFQVendorDTOPost rfqVendor);
        Task<ApiResponse<string>> DeleteRFQVendor(int id);
        Task<ApiResponse<RFQVendorDTOGet>> GetRFQVendorById(int id);
        Task<ApiResponse<List<RFQVendorDTOGet>>> GetRFQVendor(int page, int pagesize);
        Task<ApiResponse<List<RFQVendorDTOGet>>> GetSubmittedRFQForVendor(int vendorId);
        Task<ApiResponse<List<RFQVendorDTOGet>>> GetPendingRFQForVendor(int vendorId);
    }
}