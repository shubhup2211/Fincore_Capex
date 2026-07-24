using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;
using Fincore.Domain.Enums;

namespace Fincore.Application.Interfaces.IPayment
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(PaymentPostDTO dto);


        Task<ApiResponse<List<PaymentGetDTO>>> GetAllPayment(int page, int pageSize);

        Task<ApiResponse<PaymentGetDTO>> GetPaymentById(int id);

        Task UpdatePaymentAsync(int id, PaymentUpdateDTO dto);

        Task DeletePaymentAsync(int id);

        Task<ApiResponse<List<PaymentGetDTO>>> GetPaymentType(PaymentType pt,int page, int pageSize);

        Task<ApiResponse<List<PaymentGetDTO>>> GetPaymentStatus(PaymentStatus ps, int page, int pageSize);

        Task UpdateApproval(int id);

        Task UpdateReconcile(int id);

    }
}
