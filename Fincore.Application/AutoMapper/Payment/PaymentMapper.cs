using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Domain.Models;
using Fincore.Application.DTO.Payment;


public class PaymentMapper : Profile
{
    public PaymentMapper()
    {
       
        CreateMap<PaymentPostDTO, Payment>().ReverseMap();
        CreateMap<PaymentGetDTO, Payment>().ReverseMap();
        CreateMap<PaymentUpdateDTO, Payment>().ReverseMap();
    }
}

