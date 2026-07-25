using AutoMapper;
using Fincore.Application.DTO.GeneralLedger;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.AutoMapper
{
     public class GeneralLedgerProfile : Profile
    {
        public GeneralLedgerProfile()
        {
            CreateMap<JournalEntry, GeneralLedgerReadDTO>()
            .ForMember(dest => dest.AccountName,opt => opt.MapFrom(src => src.AccountMaster.AccountName));

            CreateMap<JournalEntry, LedgerAccountReadDTO>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountMaster.AccountName));

            CreateMap<JournalEntry, AccountingReportReadDTO>()
            .ForMember(dest => dest.AccountCode,opt => opt.MapFrom(src => src.AccountMaster.AccountCode))
            .ForMember(dest => dest.AccountName,opt => opt.MapFrom(src => src.AccountMaster.AccountName));
        }
        
    }
}
