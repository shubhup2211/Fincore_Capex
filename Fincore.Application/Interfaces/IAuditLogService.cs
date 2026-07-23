using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();

        Task<AuditLog?> GetByIdAsync(long id);

        Task<AuditLog> CreateAsync(AuditLog auditLog);

        Task<AuditLog?> UpdateAsync(long id, AuditLog auditLog);

        Task<bool> DeleteAsync(long id);
    }
}
