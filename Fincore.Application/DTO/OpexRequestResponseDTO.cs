namespace Fincore.Application.DTOs.OpexRequest
{
    public class OpexRequestResponseDTO
    {
        public int OpexRequestId { get; set; }

        public int BudgetLineId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        public string? ApprovalStatus { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}