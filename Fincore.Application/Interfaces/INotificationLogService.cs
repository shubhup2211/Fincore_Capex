using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;

namespace Fincore.Application.Interfaces
{
    public interface INotificationLogService
    {
        Task<PagedResponse<NotificationLogResponseDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<NotificationLogResponseDto?> GetByIdAsync(long id);

        Task<NotificationLogResponseDto> CreateAsync(NotificationLogRequestDto dto);

        Task<NotificationLogResponseDto?> UpdateAsync(long id, NotificationLogRequestDto dto);

        Task<bool> DeleteAsync(long id);
    }
}