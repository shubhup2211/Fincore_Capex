using System;
using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs
{
    public class NotificationLogRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}