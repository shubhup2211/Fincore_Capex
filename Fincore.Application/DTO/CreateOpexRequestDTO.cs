using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs.OpexRequest
{
    public class CreateOpexRequestDTO
    {
        [Required]
        public int BudgetLineId { get; set; }

        [Required]
        [StringLength(30)]
        public required string Title { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int RequestedBy { get; set; }
    }
}