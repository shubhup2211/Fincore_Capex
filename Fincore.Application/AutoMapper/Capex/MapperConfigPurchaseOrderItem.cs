using AutoMapper;
using Fincore.Application.DTO.Capex;
using Fincore.Domain.Models;

namespace Fincore.Application.AutoMapper.Capex
{
    public class MapperConfigPurchaseOrderItem : Profile
    {
        public MapperConfigPurchaseOrderItem()
        {
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDTO>()
                .ReverseMap()
                .ForMember(x => x.POItemId, y => y.Ignore());
        }
    }
}