using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO2
{
    public class PRItemDTOGet2
    {
        public int PRItemId { get; set; }
        public int PurchaseRequisitionId { get; set; }
        public string ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMaterial { get; set; }
        public decimal? EstimatedUnitPrice { get; set; }
        public decimal? LineTotal { get; set; }
    }
}
