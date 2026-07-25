using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        [StringLength(30)]
        public string VendorCode { get; set; }

        [Required]
        [ForeignKey("VendorCategory")]
        public int VendorCategoryId { get; set; }
        public VendorCategory VendorCategory { get; set; }

        [Required]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [StringLength(30)]
        public string BankAccount { get; set; }

        [StringLength(25)]
        public string PAN { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PerformanceScore { get; set; }

        public byte? IsVerified { get; set; }

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
        public List<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public List<RFQ> RFQs { get; set; }
        public List<RFQVendor> RFQVendors { get; set; }
        public List<Quotation> Quotations { get; set; }
        public List<VendorSelection> VendorSelections { get; set; }
        public List<GRN> GRNs { get; set; }
        public List<Asset> Assets { get; set; }
        public List<WorkOrder> WorkOrders { get; set; }
        public List<APInvoice> APInvoices { get; set; }
        public List<Payment> Payments { get; set; }

        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
