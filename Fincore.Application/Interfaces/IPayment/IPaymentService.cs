using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;

namespace Fincore.Application.Interfaces.IPayment
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(PaymentPostDTO dto);

    }
}
