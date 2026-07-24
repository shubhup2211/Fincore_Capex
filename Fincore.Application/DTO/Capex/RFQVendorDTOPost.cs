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
    public class RFQVendorDTOPost
    {
        public int RFQVendorId { get; set; }
        public int RFQId { get; set; }
        public int VendorId { get; set; }
        public DateTime? InvitedAt { get; set; }
        public string ResponseStatus { get; set; }

    }
}
