using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.RevenueEntry.Responses
{
    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal PendingRevenue { get; set; }

        public decimal InvoicedRevenue { get; set; }

        public decimal ReceivedRevenue { get; set; }

        public int TotalTransactions { get; set; }

        public int PendingTransactions { get; set; }

        public int InvoicedTransactions { get; set; }

        public int ReceivedTransactions { get; set; }
    }
}
