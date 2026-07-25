using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;

namespace Fincore.Application.Interfaces
{
    public interface IApprovalLogService
    {
        Task<PagedResponse<ApprovalLogResponseDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<ApprovalLogResponseDto?> GetByIdAsync(int id);

        Task<ApprovalLogResponseDto> CreateAsync(ApprovalLogRequestDto dto);

        Task<ApprovalLogResponseDto?> UpdateAsync(int id, ApprovalLogRequestDto dto);

        Task<bool> DeleteAsync(int id);
    }
}