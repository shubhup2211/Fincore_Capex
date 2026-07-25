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
    public class ApprovalLogService : IApprovalLogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ApprovalLogService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResponse<ApprovalLogResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            string cacheKey = $"ApprovalLogs_{pageNumber}_{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PagedResponse<ApprovalLogResponseDto>? cachedResult))
            {
                return cachedResult!;
            }

            var query = _context.ApprovalLogs
                .AsNoTracking()
                .OrderByDescending(x => x.ActionDate);

            int totalRecords = await query.CountAsync();

            var logs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResponse<ApprovalLogResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = _mapper.Map<List<ApprovalLogResponseDto>>(logs)
            };

            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return result;
        }

        public async Task<ApprovalLogResponseDto?> GetByIdAsync(int id)
        {
            string cacheKey = $"ApprovalLog_{id}";

            if (_cache.TryGetValue(cacheKey, out ApprovalLogResponseDto? cachedLog))
            {
                return cachedLog;
            }

            var entity = await _context.ApprovalLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ApprovalLogId == id);

            if (entity == null)
                return null;

            var result = _mapper.Map<ApprovalLogResponseDto>(entity);

            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return result;
        }

        public async Task<ApprovalLogResponseDto> CreateAsync(ApprovalLogRequestDto dto)
        {
            var entity = _mapper.Map<ApprovalLog>(dto);

            _context.ApprovalLogs.Add(entity);
            await _context.SaveChangesAsync();

            ClearCache(entity.ApprovalLogId);

            return _mapper.Map<ApprovalLogResponseDto>(entity);
        }

        public async Task<ApprovalLogResponseDto?> UpdateAsync(int id, ApprovalLogRequestDto dto)
        {
            var entity = await _context.ApprovalLogs
                .FirstOrDefaultAsync(x => x.ApprovalLogId == id);

            if (entity == null)
                return null;

            entity.EntityName = dto.EntityName ?? string.Empty;
            entity.EntityId = dto.EntityId;
            entity.ApproverId = dto.ApproverId;
            entity.Status = dto.Status ?? string.Empty;
            entity.Remarks = dto.Remarks ?? string.Empty;
            entity.ActionDate = dto.ActionDate;

            await _context.SaveChangesAsync();

            ClearCache(id);

            return _mapper.Map<ApprovalLogResponseDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ApprovalLogs
                .FirstOrDefaultAsync(x => x.ApprovalLogId == id);

            if (entity == null)
                return false;

            _context.ApprovalLogs.Remove(entity);

            await _context.SaveChangesAsync();

            ClearCache(id);

            return true;
        }

        private void ClearCache(int id)
        {
            for (int page = 1; page <= 100; page++)
            {
                for (int size = 5; size <= 100; size += 5)
                {
                    _cache.Remove($"ApprovalLogs_{page}_{size}");
                }
            }

            _cache.Remove($"ApprovalLog_{id}");
        }
    }
}