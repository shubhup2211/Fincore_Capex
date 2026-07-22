using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class ExpenseClaim
    {
        [Key]
        public int ExpenseClaimId { get; set; }

        [Required]
        [StringLength(50)]
        public string ClaimNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [ForeignKey("OpexRequest")]
        public int OpexRequestId { get; set; }
        public OpexRequest OpexRequest { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public string ExpenseType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpenseAmount { get; set; }

        [Required]
        [ForeignKey("ClaimByUser")]
        public int ClaimBy { get; set; }
        public User ClaimByUser { get; set; }

        [Required]
        [StringLength(30)]
        public string ApprovalStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [ForeignKey("ApprovedByUser")]
        public int? ApprovedBy { get; set; }
        public User ApprovedByUser { get; set; }
    }
}
