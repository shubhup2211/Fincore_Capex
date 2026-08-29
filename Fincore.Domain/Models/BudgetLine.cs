using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class BudgetLine
    {
        [Key]
        public int BudgetLineId { get; set; }

        [Required]
        [ForeignKey("Budget")]
        public int BudgetId { get; set; }
        public Budget Budget { get; set; }

        [Required]
        [ForeignKey("VendorCategory")]
        public int VendorCategoryId { get; set; }
        public VendorCategory VendorCategory { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocatedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UtilizedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RemainingAmount { get; set; }

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

        // Navigation Properties
        public List<CapexRequest> CapexRequests { get; set; }
        public List<OpexRequest> OpexRequests { get; set; }
    }
}
