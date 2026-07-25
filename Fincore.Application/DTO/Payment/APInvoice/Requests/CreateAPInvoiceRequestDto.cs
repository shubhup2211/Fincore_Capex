using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.APInvoice.Requests
{
    public class CreateAPInvoiceRequestDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int VendorId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PurchaseOrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int GRNId { get; set; }

        public int? WorkOrderId { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? InvoiceFile { get; set; }
    }
}
