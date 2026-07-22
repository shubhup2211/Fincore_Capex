using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class AuditLog
    {
        [Key]
        public long AuditLogId { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; }

        [Required]
        public long EntityId { get; set; }

        [Required]
        [StringLength(40)]
        public string OperationType { get; set; }

        public string OldData { get; set; }
        public string NewData { get; set; }

        [Required]
        [ForeignKey("AuditByUser")]
        public int AuditBy { get; set; }
        public User AuditByUser { get; set; }

        [Required]
        public DateTime AuditAt { get; set; }
    }
}
