using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment.RevenueEntry.Requests
{
    public class CreateRevenueEntryRequestDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string RevenueType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime RevenueDate { get; set; }

        [Required]
        public int AccountId { get; set; }
    }
}
