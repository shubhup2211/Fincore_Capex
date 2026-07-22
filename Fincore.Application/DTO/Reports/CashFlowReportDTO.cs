using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    
        public class CashFlowReportDTO
        {
            public decimal TotalCashInflow { get; set; }

            public decimal TotalCashOutflow { get; set; }

            public decimal NetCashFlow { get; set; }
        }
    
}
