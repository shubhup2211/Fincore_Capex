using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO2
{
    public class PRDTOPost2
    {
        public int CapexRequestId { get; set; }
        public string PRTitle { get; set; }

        public decimal Amount { get; set; }

        public DateTime? RequiredTillDate { get; set; }
    }
}
