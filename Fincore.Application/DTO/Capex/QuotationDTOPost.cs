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
    public class QuotationDTOPost
    {
        public int QuotationId { get; set; }
        public int RFQId { get; set; }
        public int VendorId { get; set; }
        public string QuotationNumber { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Remarks { get; set; }
        public byte IsSelected { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
