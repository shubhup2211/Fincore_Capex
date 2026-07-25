using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;

namespace Fincore.Application.Interfaces
{
    public interface IUserActivityLogService
    {
        Task<PagedResponse<UserActivityLogResponseDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<UserActivityLogResponseDto?> GetByIdAsync(long id);

        Task<UserActivityLogResponseDto> CreateAsync(UserActivityLogRequestDto dto);

        Task<UserActivityLogResponseDto?> UpdateAsync(long id, UserActivityLogRequestDto dto);

        Task<bool> DeleteAsync(long id);
    }
}