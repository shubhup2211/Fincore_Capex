using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    namespace Fincore.Application.DTOs
    {
        public class ApprovalLogDTO
        {
            public int ApprovalLogId { get; set; }

            public string EntityName { get; set; }

            public long EntityId { get; set; }

            public int ApproverId { get; set; }

            public string Status { get; set; }

            public string? Remarks { get; set; }

            public DateTime ActionDate { get; set; }
        }
    }
