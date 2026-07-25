using System;
using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTOs
{
    public class UserActivityLogRequestDto
    {
        public int UserId { get; set; }

        public string ActivityType { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;

        public DateTime ActivityDate { get; set; }
    }

}