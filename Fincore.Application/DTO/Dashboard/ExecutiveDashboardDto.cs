using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Dashboard
{
    public class ExecutiveDashboardDto
    {
            public decimal TotalRevenue { get; set; }
            public int TotalInvoices { get; set; }
            public decimal ReceivedRevenue { get; set; }
            public decimal PendingRevenue { get; set; }
            public int InvoiceCount { get; set; }
            public int ReceivedCount { get; set; }
            public int PendingCount { get; set; }
        
    }
}
