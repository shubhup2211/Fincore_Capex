using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class POTotalDTO
    {
        public int POId { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
