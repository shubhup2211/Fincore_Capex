using System;

namespace Fincore.Application.DTOs
{
    public class UserActivityLogResponseDto
    {
        public long UserActivityLogId { get; set; }

        public int UserId { get; set; }

        public string ActivityType { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }
    }
}