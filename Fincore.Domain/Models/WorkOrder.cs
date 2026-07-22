using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class WorkOrder
    {
        [Key]
        public int WorkOrderId { get; set; }

        [Required]
        [StringLength(30)]
        public string WONumber { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        [Required]
        [ForeignKey("OpexRequest")]
        public int OpexRequestId { get; set; }
        public OpexRequest OpexRequest { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        public DateTime? CreatedDate { get; set; }

        // Navigation Properties
        public List<APInvoice> APInvoices { get; set; }
    }
}
