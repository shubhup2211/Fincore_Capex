using AutoMapper;
using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public AuditLogService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<PagedResponse<AuditLogResponseDto>> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            string cacheKey =
                $"auditlogs_{pageNumber}_{pageSize}";
            if (_cache.TryGetValue(cacheKey,
                out PagedResponse<AuditLogResponseDto>? cachedData))
            {
                return cachedData!;
            }
            var query = _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.AuditLogId);
            var totalRecords = await query.CountAsync();
            var logs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var response = new PagedResponse<AuditLogResponseDto>
            {
                PageNumber = pageNumber,

                PageSize = pageSize,

                TotalRecords = totalRecords,

                TotalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize),


                Data = _mapper.Map<List<AuditLogResponseDto>>(logs)

            };

            _cache.Set(
                cacheKey,
                response,
                TimeSpan.FromMinutes(5)
            );
            return response;

        }
        public async Task<AuditLogResponseDto?> GetByIdAsync(long id)
        {
            string cacheKey = $"auditlog_{id}";
            if (_cache.TryGetValue(cacheKey,
                out AuditLogResponseDto? cached))
            {
                return cached;
            }

            var log = await _context.AuditLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.AuditLogId == id);



            if (log == null)
                return null;


            var result = _mapper.Map<AuditLogResponseDto>(log);
            _cache.Set(
                cacheKey,
                result,
                TimeSpan.FromMinutes(5)
            );
            return result;

        }
        public async Task<AuditLogResponseDto> CreateAsync(
            AuditLogRequestDto dto)
        {
            var audit =
                _mapper.Map<Domain.Models.AuditLog>(dto);
            _context.AuditLogs.Add(audit);  
            await _context.SaveChangesAsync();
            var response =
                _mapper.Map<AuditLogResponseDto>(audit);
            return response;

        }
        public async Task<AuditLogResponseDto?> UpdateAsync(
            long id,
            AuditLogRequestDto dto)
        {


            var existing =
                await _context.AuditLogs
                .FirstOrDefaultAsync(x =>
                x.AuditLogId == id);

            if (existing == null)
                return null;

            _mapper.Map(dto, existing);


            await _context.SaveChangesAsync();



            _cache.Remove($"auditlog_{id}");



            return _mapper.Map<AuditLogResponseDto>(existing);
        }
        public async Task<bool> DeleteAsync(long id)
        {

            var audit =
                await _context.AuditLogs
                .FindAsync(id);
            if (audit == null)
                return false;
            _context.AuditLogs.Remove(audit);
            await _context.SaveChangesAsync();
            _cache.Remove($"auditlog_{id}");
            return true;

        }

    }
}