

namespace Fincore.Application.DTOs
{
    public class NotificationLogDTO
    {
        public long NotificationLogId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; }
    }
}