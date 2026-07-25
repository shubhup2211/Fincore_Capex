namespace Fincore.Application.DTOs.BudgetCategory
{
    public class BudgetCategoryResponseDTO
    {
        public int BudgetCategoryId { get; set; }

        public string CategoryName { get; set; }

        public int DepartmentId { get; set; }

        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}