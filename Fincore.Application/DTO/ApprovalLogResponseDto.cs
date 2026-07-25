using System;

namespace Fincore.Application.DTOs
{
    public class ApprovalLogResponseDto
    {
        public int ApprovalLogId { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public long EntityId { get; set; }

        public int ApproverId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime ActionDate { get; set; }
    }
}