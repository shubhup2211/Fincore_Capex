using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment
{
    public class APInvoiceDTO
    {

        [Key]
        public int APInvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        [Required]
        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        [Required]
        [ForeignKey("GRN")]
        public int GRNId { get; set; }
        public GRN GRN { get; set; }

        [ForeignKey("WorkOrder")]
        public int? WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string InvoiceFile { get; set; }

        [ForeignKey("ApprovedByUser")]
        public int? ApprovedBy { get; set; }
        public User ApprovedByUser { get; set; }

        [Required]
        [StringLength(20)]
        public string ApprovalStatus { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation Properties
       //public Payment Payments { get; set; }










    }
}
