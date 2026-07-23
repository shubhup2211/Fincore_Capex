using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class NotificationLogService : INotificationLogService
    {
        private readonly AppDbContext _context;

        public NotificationLogService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<NotificationLog>> GetAllAsync()
        {
            return await _context.NotificationLogs
                .Include(x => x.User)
                .ToListAsync();
        }


        public async Task<NotificationLog?> GetByIdAsync(long id)
        {
            return await _context.NotificationLogs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.NotificationLogId == id);
        }


        public async Task<NotificationLog> CreateAsync(NotificationLog log)
        {
            _context.NotificationLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }


        public async Task<NotificationLog?> UpdateAsync(long id, NotificationLog log)
        {
            var existing = await _context.NotificationLogs.FindAsync(id);

            if (existing == null)
                return null;

            existing.UserId = log.UserId;
            existing.Title = log.Title;
            existing.Message = log.Message;
            existing.SentAt = log.SentAt;

            await _context.SaveChangesAsync();

            return existing;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var log = await _context.NotificationLogs.FindAsync(id);

            if (log == null)
                return false;

            _context.NotificationLogs.Remove(log);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}