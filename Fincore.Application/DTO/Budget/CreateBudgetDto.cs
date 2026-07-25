using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs.Budget
{
    public class CreateBudgetDTO
    {
        [Required]
        [StringLength(20)]
        public required string BudgetCode { get; set; }

        [Required]
        [StringLength(30)]
        public required string BudgetName { get; set; }

        [Required]
        [StringLength(20)]
        public required string FinancialYear { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public decimal BudgetAmount { get; set; }

        [Required]
        public byte IsActive { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int ModifiedBy { get; set; }
        // created at and modified at should be inclded in servce , not sent by the client therefore not included .
    }
}