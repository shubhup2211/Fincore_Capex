using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;

namespace Fincore.Application.Interfaces
{
    public interface IAuditLogService
    {

        Task<PagedResponse<AuditLogResponseDto>>
        GetAllAsync(int pageNumber, int pageSize);


        Task<AuditLogResponseDto?>
        GetByIdAsync(long id);


        Task<AuditLogResponseDto>
        CreateAsync(AuditLogRequestDto dto);


        Task<AuditLogResponseDto?>
        UpdateAsync(long id, AuditLogRequestDto dto);


        Task<bool> DeleteAsync(long id);

    }
}