using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class ApprovalLog
    {
        [Key]
        public int ApprovalLogId { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; }

        [Required]
        public long EntityId { get; set; }

        [Required]
        [ForeignKey("ApproverUser")]
        public int ApproverId { get; set; }
        public User ApproverUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }
    }
}
