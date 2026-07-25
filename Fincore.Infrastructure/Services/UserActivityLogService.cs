using AutoMapper;
using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public UserActivityLogService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResponse<UserActivityLogResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            string cacheKey = $"UserActivityLogs_{pageNumber}_{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedResponse<UserActivityLogResponseDto>? cachedResult))
            {
                return cachedResult!;
            }

            var query = _context.UserActivityLogs
                .AsNoTracking()
                .OrderByDescending(x => x.ActivityDate);

            int totalRecords = await query.CountAsync();

            var logs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResponse<UserActivityLogResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = _mapper.Map<List<UserActivityLogResponseDto>>(logs)
            };

            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return result;
        }

        public async Task<UserActivityLogResponseDto?> GetByIdAsync(long id)
        {
            string cacheKey = $"UserActivityLog_{id}";

            if (_cache.TryGetValue(cacheKey, out UserActivityLogResponseDto? cachedLog))
            {
                return cachedLog;
            }

            var entity = await _context.UserActivityLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserActivityLogId == id);

            if (entity == null)
                return null;

            var result = _mapper.Map<UserActivityLogResponseDto>(entity);

            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return result;
        }

        public async Task<UserActivityLogResponseDto> CreateAsync(UserActivityLogRequestDto dto)
        {
            var entity = _mapper.Map<UserActivityLog>(dto);

            _context.UserActivityLogs.Add(entity);
            await _context.SaveChangesAsync();

            ClearCache();

            return _mapper.Map<UserActivityLogResponseDto>(entity);
        }

        public async Task<UserActivityLogResponseDto?> UpdateAsync(long id, UserActivityLogRequestDto dto)
        {
            var entity = await _context.UserActivityLogs
                .FirstOrDefaultAsync(x => x.UserActivityLogId == id);

            if (entity == null)
                return null;

            entity.UserId = dto.UserId;
            entity.ActivityType = dto.ActivityType;
            entity.Module = dto.Module;
            entity.ActivityDate = dto.ActivityDate;

            await _context.SaveChangesAsync();

            ClearCache();

            return _mapper.Map<UserActivityLogResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.UserActivityLogs
                .FirstOrDefaultAsync(x => x.UserActivityLogId == id);

            if (entity == null)
                return false;

            _context.UserActivityLogs.Remove(entity);
            await _context.SaveChangesAsync();

            ClearCache();

            return true;
        }

        private void ClearCache()
        {
            for (int page = 1; page <= 100; page++)
            {
                for (int size = 5; size <= 100; size += 5)
                {
                    _cache.Remove($"UserActivityLogs_{page}_{size}");
                }
            }

            _cache.Remove("UserActivityLog");
        }
    }
}