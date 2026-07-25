namespace Fincore.Application.DTOs.BudgetLine
{
    public class BudgetLineResponseDTO
    {
        public int BudgetLineId { get; set; }

        public int BudgetId { get; set; }

        public int BudgetCategoryId { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal? UtilizedAmount { get; set; }

        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }
    }
}