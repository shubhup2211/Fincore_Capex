using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class OpexRequest
    {
        [Key]
        public int OpexRequestId { get; set; }

        [Required]
        [ForeignKey("BudgetLine")]
        public int BudgetLineId { get; set; }
        public BudgetLine BudgetLine { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [ForeignKey("RequestedByUser")]
        public int RequestedBy { get; set; }
        public User RequestedByUser { get; set; }

        [Required]
        [StringLength(15)]
        public string ApprovalStatus { get; set; }

        [ForeignKey("ApprovedByUser")]
        public int? ApprovedBy { get; set; }
        public User ApprovedByUser { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation Properties
        public List<ExpenseClaim> ExpenseClaims { get; set; }
        public List<WorkOrder> WorkOrders { get; set; }
    }
}
