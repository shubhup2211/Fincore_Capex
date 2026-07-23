using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTOs
{
    public class UserActivityLogDTO
    {
        public long UserActivityLogId { get; set; }

        public int UserId { get; set; }

        public string ActivityType { get; set; }

        public string? Module { get; set; }

        public DateTime ActivityDate { get; set; }
    }
}