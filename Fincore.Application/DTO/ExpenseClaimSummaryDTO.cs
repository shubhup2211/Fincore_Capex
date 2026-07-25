namespace Fincore.Application.DTOs.ExpenseClaim
{
    public class ExpenseClaimSummaryDTO
    {
        public int TotalClaims { get; set; }

        public int ApprovedClaims { get; set; }

        public int RejectedClaims { get; set; }

        public int PendingClaims { get; set; }

        public decimal TotalExpenseAmount { get; set; }
    }
}