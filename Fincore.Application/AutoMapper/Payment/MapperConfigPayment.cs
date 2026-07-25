using AutoMapper;
using Fincore.Application.DTO.Payment.RevenueEntry.Requests;
using Fincore.Application.DTO.Payment.RevenueEntry.Responses;
using Fincore.Domain.Models;
using System;


namespace Fincore.Application.AutoMapper.Payment
{
    public class MapperConfigPayment : Profile
    {
        public MapperConfigPayment()
        {
            // Create
            CreateMap<CreateRevenueEntryRequestDto, RevenueEntry>();

            // Update
            CreateMap<UpdateRevenueEntryRequestDto, RevenueEntry>();

            // Response
            CreateMap<RevenueEntry, RevenueEntryResponseDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.CustomerCode))

                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department.DepartmentName))

                .ForMember(dest => dest.AccountName,
                    opt => opt.MapFrom(src => src.AccountMaster.AccountName));
        }
    }
}
