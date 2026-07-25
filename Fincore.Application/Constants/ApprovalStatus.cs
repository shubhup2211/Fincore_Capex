using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Constants
{
    public static class ApprovalStatus
    {
        public const string Draft = "Draft";

        public const string Approved = "Approved";

        public const string Rejected = "Rejected";

        public const string Cancelled = "Cancelled";

        public const string Closed = "Closed";
    }
}
