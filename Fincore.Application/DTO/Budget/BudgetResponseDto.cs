using System;

namespace Fincore.Application.DTOs.Budget
{
    public class BudgetResponseDTO
    {
        public int BudgetId { get; set; }

        public string BudgetCode { get; set; }

        public string BudgetName { get; set; }

        public string FinancialYear { get; set; }

        public int BudgetCategoryId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal BudgetAmount { get; set; }

        public byte IsActive { get; set; }
    }
}