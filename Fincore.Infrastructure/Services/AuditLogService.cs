using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(x => x.AuditByUser)
                .ToListAsync();
        }


        public async Task<AuditLog?> GetByIdAsync(long id)
        {
            return await _context.AuditLogs
                .Include(x => x.AuditByUser)
                .FirstOrDefaultAsync(x => x.AuditLogId == id);
        }


        public async Task<AuditLog> CreateAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            return auditLog;
        }


        public async Task<AuditLog?> UpdateAsync(long id, AuditLog auditLog)
        {
            var existing = await _context.AuditLogs
                .FindAsync(id);

            if (existing == null)
                return null;


            existing.EntityName = auditLog.EntityName;
            existing.EntityId = auditLog.EntityId;
            existing.OperationType = auditLog.OperationType;
            existing.OldData = auditLog.OldData;
            existing.NewData = auditLog.NewData;
            existing.AuditBy = auditLog.AuditBy;
            existing.AuditAt = auditLog.AuditAt;


            await _context.SaveChangesAsync();

            return existing;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var auditLog = await _context.AuditLogs
                .FindAsync(id);

            if (auditLog == null)
                return false;


            _context.AuditLogs.Remove(auditLog);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}