using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class ApprovalFlow
    {
        [Key]
        public int ApprovalFlowId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxAmount { get; set; }

        [Required]
        public int ApprovalLevel { get; set; }

        [Required]
        [ForeignKey("RequiredRole")]
        public int RequiredRoleId { get; set; }
        public Role RequiredRole { get; set; }

        [Required]
        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        [ForeignKey("ModifiedByUser")]
        public int ModifiedBy { get; set; }
        public User ModifiedByUser { get; set; }

        public List<CapexRequest> CapexRequests { get; set; }
    }
}
