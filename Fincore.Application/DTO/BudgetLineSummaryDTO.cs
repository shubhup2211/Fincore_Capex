namespace Fincore.Application.DTOs.BudgetLine
{
    public class BudgetLineSummaryDTO
    {
        public int TotalBudgetLines { get; set; }

        public decimal TotalAllocatedAmount { get; set; }

        public decimal TotalUtilizedAmount { get; set; }

        public int ActiveBudgetLines { get; set; }

        public int InactiveBudgetLines { get; set; }
    }
}