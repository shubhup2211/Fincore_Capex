using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Reports
{
    public class BudgetVarianceReportDTO
    {
        public int BudgetLineId { get; set; }

        public string BudgetCode { get; set; }

        public string BudgetName { get; set; }

        public string FinancialYear { get; set; }

        public string CategoryName { get; set; }

        public string DepartmentName { get; set; }

        public decimal BudgetAmount { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal UtilizedAmount { get; set; }

        public decimal RemainingAmount { get; set; }
    }
}
