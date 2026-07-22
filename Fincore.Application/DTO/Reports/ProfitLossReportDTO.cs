using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
     public class ProfitLossReportDTO
    {
        
            public decimal TotalRevenue { get; set; }

            public decimal TotalExpense { get; set; }

            public decimal NetProfitOrLoss { get; set; }
        
        
    }
}
