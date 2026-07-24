using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Dashboard
{
    public class FinanceDashboardDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal TotalPayments { get; set; }
        public decimal ApprovedPayments { get; set; }
        public decimal PendingPayments { get; set; }

        public int TotalPurchaseOrders { get; set; }
        public int ApprovedPurchaseOrders { get; set; }
        public int PendingPurchaseOrders { get; set; }
        public decimal PurchaseOrderAmount { get; set; }

        public int TotalPurchaseRequisitions { get; set; }
        public int ApprovedPurchaseRequisitions { get; set; }
        public int PendingPurchaseRequisitions { get; set; }
        public decimal PurchaseRequisitionAmount { get; set; }
    }
}
