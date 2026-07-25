using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    
        public class CashFlowReportDTO
        {
        public string CompanyName { get; set; }

        public string DepartmentName { get; set; }

        public decimal CashInflow { get; set; }

        public decimal CashOutflow { get; set; }

        public decimal NetCashFlow { get; set; }

        public string CashFlowStatus { get; set; }
    }
    
}
