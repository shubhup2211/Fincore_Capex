using System;
using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs
{
    public class ApprovalLogRequestDto
    {
        [Required]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        public long EntityId { get; set; }

        [Required]
        public int ApproverId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}