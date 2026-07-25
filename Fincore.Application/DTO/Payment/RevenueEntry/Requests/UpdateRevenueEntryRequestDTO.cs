using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.RevenueEntry.Requests
{
    public class UpdateRevenueEntryRequestDto
    {
        public int CustomerId { get; set; }
        public int DepartmentId { get; set; }
        public string RevenueType { get; set; }
        public decimal Amount { get; set; }
        public DateTime RevenueDate { get; set; }
        public int AccountId { get; set; }
    }
}
