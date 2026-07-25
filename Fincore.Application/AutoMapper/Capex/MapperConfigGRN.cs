using AutoMapper;
using Fincore.Application.DTO.Capex;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.AutoMapper.Capex
{
    public class MapperConfigGRN :Profile
    {
        public MapperConfigGRN()
        {
            CreateMap<GRN, GRNDTO>().ReverseMap().ForMember(x => x.GRNId, y => y.Ignore());
        }
    }
}
