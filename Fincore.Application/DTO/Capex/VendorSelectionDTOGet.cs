using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class VendorSelectionDTOGet
    {
        public int VendorSelectionId { get; set; }
        public string RFQTitle { get; set; }
        public string QuotationNumber { get; set; }
        public string SelectedVendorCode { get; set; }
        public DateTime? SelectedDate { get; set; }
        public string SelectedBy { get; set; }
        public string Remarks { get; set; }
    }
}
