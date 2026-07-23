using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class AuditLogDTO
    {
        public long AuditLogId { get; set; }

        public string EntityName { get; set; }

        public long EntityId { get; set; }

        public string OperationType { get; set; }

        public string? OldData { get; set; }

        public string? NewData { get; set; }

        public int AuditBy { get; set; }

        public DateTime AuditAt { get; set; }
    }
}
