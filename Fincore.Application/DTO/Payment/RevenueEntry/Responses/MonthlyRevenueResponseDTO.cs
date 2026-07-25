using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.RevenueEntry.Responses
{
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public string MonthName { get; set; }

        public decimal TotalRevenue { get; set; }

        public int TotalTransactions { get; set; }
    }
}
