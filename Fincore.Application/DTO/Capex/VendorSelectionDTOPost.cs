using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class VendorSelectionDTOPost
    {
        public int VendorSelectionId { get; set; }
        public int RFQId { get; set; }
        public int QuotationId { get; set; }
        public int SelectedVendorId { get; set; }
        public DateTime? SelectedDate { get; set; }
        public int? SelectedBy { get; set; }
        public string Remarks { get; set; }
    }
}
