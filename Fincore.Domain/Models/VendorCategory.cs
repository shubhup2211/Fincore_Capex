using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class VendorCategory
    {
        [Key]
        public int VendorCategoryId { get; set; }

        [Required]
        [StringLength(30)]
        public string CategoryName { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

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
        public List<Vendor> Vendors { get; set; }
        public List<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public List<BudgetLine> BudgetLines { get; set; }
    }
}
