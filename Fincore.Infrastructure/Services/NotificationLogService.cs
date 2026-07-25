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
    public class NotificationLogService : INotificationLogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public NotificationLogService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResponse<NotificationLogResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            string cacheKey = $"NotificationLogs_{pageNumber}_{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedResponse<NotificationLogResponseDto>? cachedResult))
            {
                return cachedResult!;
            }

            var query = _context.NotificationLogs
                .AsNoTracking()
                .OrderByDescending(x => x.SentAt);

            int totalRecords = await query.CountAsync();

            var logs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResponse<NotificationLogResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = _mapper.Map<List<NotificationLogResponseDto>>(logs)
            };

            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return result;
        }

        public async Task<NotificationLogResponseDto?> GetByIdAsync(long id)
        {
            string cacheKey = $"NotificationLog_{id}";

            if (_cache.TryGetValue(cacheKey, out NotificationLogResponseDto? cachedLog))
            {
                return cachedLog;
            }

            var entity = await _context.NotificationLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NotificationLogId == id);

            if (entity == null)
                return null;

            var result = _mapper.Map<NotificationLogResponseDto>(entity);

            _cache.Set(cacheKey, result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return result;
        }

        public async Task<NotificationLogResponseDto> CreateAsync(NotificationLogRequestDto dto)
        {
            var entity = _mapper.Map<NotificationLog>(dto);

            _context.NotificationLogs.Add(entity);
            await _context.SaveChangesAsync();

            ClearCache();

            return _mapper.Map<NotificationLogResponseDto>(entity);
        }

        public async Task<NotificationLogResponseDto?> UpdateAsync(long id, NotificationLogRequestDto dto)
        {
            var entity = await _context.NotificationLogs
                .FirstOrDefaultAsync(x => x.NotificationLogId == id);

            if (entity == null)
                return null;

            entity.UserId = dto.UserId;
            entity.Title = dto.Title;
            entity.Message = dto.Message;
            entity.SentAt = dto.SentAt;

            await _context.SaveChangesAsync();

            ClearCache();

            return _mapper.Map<NotificationLogResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.NotificationLogs
                .FirstOrDefaultAsync(x => x.NotificationLogId == id);

            if (entity == null)
                return false;

            _context.NotificationLogs.Remove(entity);

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
                    _cache.Remove($"NotificationLogs_{page}_{size}");
                }
            }

            _cache.Remove("NotificationLog");
        }
    }
}