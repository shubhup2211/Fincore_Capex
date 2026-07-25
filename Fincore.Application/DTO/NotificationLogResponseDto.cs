using System;

namespace Fincore.Application.DTOs
{
    public class NotificationLogResponseDto
    {
        public long NotificationLogId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }
    }
}