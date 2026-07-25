namespace Fincore.Application.DTOs.OpexRequest
{
    public class OpexSummaryDTO
    {
        public int TotalRequest { get; set; }

        public int ApprovedRequest { get; set; }

        public int RejectedRequest { get; set; }

        public int PendingRequest { get; set; }

        public decimal TotalAmount { get; set; }
    }
}