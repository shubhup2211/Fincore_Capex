using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs.BudgetLine
{
    public class UpdateBudgetLineDTO
    {
        [Required]
        public int BudgetId { get; set; }

        [Required]
        public int BudgetCategoryId { get; set; }

        [Required]
        public decimal AllocatedAmount { get; set; }

        public decimal? UtilizedAmount { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
    }
}