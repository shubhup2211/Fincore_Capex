using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Domain.Models
{
    public class VendorSelection
    {
        [Key]
        public int VendorSelectionId { get; set; }

        [Required]
        [ForeignKey("RFQ")]
        public int RFQId { get; set; }
        public RFQ RFQ { get; set; }

        [Required]
        [ForeignKey("Quotation")]
        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        [Required]
        [ForeignKey("SelectedVendor")]
        public int SelectedVendorId { get; set; }
        public Vendor SelectedVendor { get; set; }

        public DateTime? SelectedDate { get; set; }

        [ForeignKey("SelectedByUser")]
        public int? SelectedBy { get; set; }
        public User SelectedByUser { get; set; }

        [StringLength(20)]
        public string Remarks { get; set; }
    }
}
