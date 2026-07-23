using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO.Payment;
using Fincore.Domain.Models;

namespace Fincore.Application.AutoMapper.Payment
{
    internal class JournalEntryMapper : Profile
    {
        public JournalEntryMapper()
        {
            CreateMap<JournalEntryPostDTO, JournalEntry>().ReverseMap();
            CreateMap<JournalEntryGetDTO, JournalEntry>().ReverseMap();
            CreateMap<JournalEntryUpdateDTO, JournalEntry>().ReverseMap();
        }
    }
}
