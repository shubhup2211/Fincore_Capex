using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class QuotationDTOGet
    {
        public int QuotationId { get; set; }
        public string RFQTitle { get; set; }
        public string VendorCode { get; set; }
        public string QuotationNumber { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Remarks { get; set; }
        public byte IsSelected { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
