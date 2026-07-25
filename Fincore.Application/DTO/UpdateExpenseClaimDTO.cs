namespace Fincore.Application.DTOs.ExpenseClaim
{
    public class UpdateExpenseClaimDTO
    {
        public string ClaimNumber { get; set; }

        public string Description { get; set; }

        public int OpexRequestId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ExpenseType { get; set; }

        public decimal ExpenseAmount { get; set; }

        public int ClaimBy { get; set; }
    }
}