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
    public class MapperConfigPurchaseOrder : Profile
    {
        public MapperConfigPurchaseOrder()
        {
            CreateMap<PurchaseOrder, PurchaseOrderDTO>().ReverseMap().ForMember(x=>x.POId,y=>y.Ignore());
        }
    }
}
