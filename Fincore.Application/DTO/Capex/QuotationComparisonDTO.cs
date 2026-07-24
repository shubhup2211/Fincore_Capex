using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class QuotationComparisonDTO
    {
        public int QuotationId { get; set; }
        public string QuotationNumber { get; set; }
        public string CompanyName { get; set; }
        public string VendorCode { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Remarks { get; set; }
        public string SelectionStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
