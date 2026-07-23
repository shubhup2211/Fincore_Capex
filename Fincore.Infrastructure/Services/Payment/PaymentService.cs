using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Infrastructure.Data;

namespace Fincore.Infrastructure.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        IPaymentService service;
        AppDbContext db;
        IMapper mapper;

        public PaymentService(IPaymentService service, AppDbContext db, IMapper mapper)
        {
            this.service = service;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPayment(PaymentPostDTO dto)
        {
           var data=  mapper.Map<PaymentPostDTO>(dto);
           

        }
    }
}
