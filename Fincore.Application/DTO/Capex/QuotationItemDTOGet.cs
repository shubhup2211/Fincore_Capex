using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Capex
{
    public class QuotationItemDTOGet
    {
        public int QuotationItemId { get; set; }
        public string QuotationNumber { get; set; }
        public string PRItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
