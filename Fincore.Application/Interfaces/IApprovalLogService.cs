using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface IApprovalLogService
    {
        Task<IEnumerable<ApprovalLog>> GetAllAsync();

        Task<ApprovalLog?> GetByIdAsync(int id);

        Task<ApprovalLog> CreateAsync(ApprovalLog log);

        Task<ApprovalLog?> UpdateAsync(int id, ApprovalLog log);

        Task<bool> DeleteAsync(int id);
    }
}
