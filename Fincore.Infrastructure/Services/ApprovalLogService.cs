using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class ApprovalLogService : IApprovalLogService
    {
        private readonly AppDbContext _context;

        public ApprovalLogService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ApprovalLog>> GetAllAsync()
        {
            return await _context.ApprovalLogs
                .Include(x => x.ApproverUser)
                .ToListAsync();
        }


        public async Task<ApprovalLog?> GetByIdAsync(int id)
        {
            return await _context.ApprovalLogs
                .Include(x => x.ApproverUser)
                .FirstOrDefaultAsync(x => x.ApprovalLogId == id);
        }


        public async Task<ApprovalLog> CreateAsync(ApprovalLog log)
        {
            _context.ApprovalLogs.Add(log);

            await _context.SaveChangesAsync();

            return log;
        }


        public async Task<ApprovalLog?> UpdateAsync(int id, ApprovalLog log)
        {
            var existing = await _context.ApprovalLogs
                .FindAsync(id);

            if (existing == null)
                return null;


            existing.EntityName = log.EntityName;
            existing.EntityId = log.EntityId;
            existing.ApproverId = log.ApproverId;
            existing.Status = log.Status;
            existing.Remarks = log.Remarks;
            existing.ActionDate = log.ActionDate;


            await _context.SaveChangesAsync();

            return existing;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var log = await _context.ApprovalLogs
                .FindAsync(id);

            if (log == null)
                return false;


            _context.ApprovalLogs.Remove(log);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}