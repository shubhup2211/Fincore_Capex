using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class RFQVendorDTOGet
    {
        public int RFQVendorId { get; set; }
        public string RFQTitle { get; set; }
        public string VendorCode { get; set; }
        public DateTime? InvitedAt { get; set; }
        public string ResponseStatus { get; set; }
    }
}
