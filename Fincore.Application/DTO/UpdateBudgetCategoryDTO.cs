namespace Fincore.Application.DTOs.BudgetCategory
{
    public class UpdateBudgetCategoryDTO
    {
        public string CategoryName { get; set; }

        public int DepartmentId { get; set; }

        public byte IsActive { get; set; }

        public int ModifiedBy { get; set; }
    }
}