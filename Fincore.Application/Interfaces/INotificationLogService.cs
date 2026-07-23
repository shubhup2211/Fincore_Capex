using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface INotificationLogService
    {
        Task<IEnumerable<NotificationLog>> GetAllAsync();
        Task<NotificationLog?> GetByIdAsync(long id);
        Task<NotificationLog> CreateAsync(NotificationLog log);
        Task<NotificationLog?> UpdateAsync(long id, NotificationLog log);
        Task<bool> DeleteAsync(long id);
    }
}