using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
     public class ProfitLossReportDTO
    {
        public string CompanyName { get; set; }

        public string DepartmentName { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal Profit { get; set; }

        public decimal Loss { get; set; }

    }
}
