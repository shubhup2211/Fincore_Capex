using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.DTO.Payment
{
    public class PaymentUpdateDTO
    {
        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; }
    }
}
