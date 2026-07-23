using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly AppDbContext _context;

        public UserActivityLogService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<UserActivityLog>> GetAllAsync()
        {
            return await _context.UserActivityLogs
                .Include(x => x.User)
                .ToListAsync();
        }


        public async Task<UserActivityLog?> GetByIdAsync(long id)
        {
            return await _context.UserActivityLogs
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserActivityLogId == id);
        }


        public async Task<UserActivityLog> CreateAsync(UserActivityLog log)
        {
            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }


        public async Task<UserActivityLog?> UpdateAsync(long id, UserActivityLog log)
        {
            var existing = await _context.UserActivityLogs.FindAsync(id);

            if (existing == null)
                return null;

            existing.UserId = log.UserId;
            existing.ActivityType = log.ActivityType;
            existing.Module = log.Module;
            existing.ActivityDate = log.ActivityDate;

            await _context.SaveChangesAsync();

            return existing;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            var log = await _context.UserActivityLogs.FindAsync(id);

            if (log == null)
                return false;

            _context.UserActivityLogs.Remove(log);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}