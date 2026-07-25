namespace Fincore.Application.DTOs.Budget
{
    public class BudgetSummaryDTO
    {
        public int TotalBudgets { get; set; }

        public int ActiveBudgets { get; set; }

        public int InactiveBudgets { get; set; }

        public decimal TotalBudgetAmount { get; set; }
    }
}