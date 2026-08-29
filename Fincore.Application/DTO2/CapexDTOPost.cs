using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO2
{
    public class CapexDTOPost
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int BudgetLineId { get; set; }
    }
}
