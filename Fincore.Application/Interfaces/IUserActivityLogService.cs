using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface IUserActivityLogService
    {
        Task<IEnumerable<UserActivityLog>> GetAllAsync();
        Task<UserActivityLog?> GetByIdAsync(long id);
        Task<UserActivityLog> CreateAsync(UserActivityLog log);
        Task<UserActivityLog?> UpdateAsync(long id, UserActivityLog log);
        Task<bool> DeleteAsync(long id);
    }
}