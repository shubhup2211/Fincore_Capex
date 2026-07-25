using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs.OpexRequest
{
    public class UpdateOpexRequestDTO
    {
        [Required]
        public int OpexRequestId { get; set; }
        
        [Required]
        public int BudgetLineId { get; set; }
         
        [Required]
        [StringLength(30)]
        public required string Title { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public int? ApprovedBy { get; set; }
        public required string ApprovalStatus { get; set; }
    }
}