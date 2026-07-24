using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Domain.Enums;

namespace Fincore.Application.DTO.Payment
{
    public class PaymentPostDTO
    {
        [Required]
        public PaymentType PaymentType { get; set; }

        public int? APInvoiceId { get; set; }

        public int? ARInvoiceId { get; set; }

        

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
