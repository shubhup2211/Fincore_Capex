using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Dashboard
{
    public class BudgetDashboardDto
    {
        public decimal TotalBudget { get; set; }
        public decimal UsedBudget { get; set; }
        public decimal RemainingBudget { get; set; }

        public int TotalBudgets { get; set; }
        public int ActiveBudgets { get; set; }
        public int InactiveBudgets { get; set; }

        public decimal BudgetUtilizationPercentage { get; set; }

        public string? CurrentFinancialYear { get; set; }
    }
}
