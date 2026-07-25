using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTOs;
using Fincore.Domain.Models;


namespace Fincore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Country

            CreateMap<CountryRequestDto, Country>();
            CreateMap<Country, CountryResponseDto>();

            // State

            CreateMap<StateRequestDto, State>();
            CreateMap<State, StateResponseDto>();

            // City

            CreateMap<CityRequestDto, City>();
            CreateMap<City, CityResponseDto>();

            // Currency Mapping

            CreateMap<CurrencyRequestDto, Currency>();
            CreateMap<Currency, CurrencyResponseDto>();

            // AuditLog Mapping

            CreateMap<AuditLogRequestDto, AuditLog>();
            CreateMap<AuditLog, AuditLogResponseDto>();

            // User Activity Logs

            CreateMap<UserActivityLogRequestDto, UserActivityLog>();
            CreateMap<UserActivityLog, UserActivityLogResponseDto>();

            // Notification Log

            CreateMap<NotificationLogRequestDto, NotificationLog>();
            CreateMap<NotificationLog, NotificationLogResponseDto>();

            // Approval Log

            CreateMap<ApprovalLogRequestDto, ApprovalLog>();
            CreateMap<ApprovalLog, ApprovalLogResponseDto>();

        }
    }
}