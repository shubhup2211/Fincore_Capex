using AutoMapper;
using Fincore.Application.DTO2;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.AutoMapper.Capex
{
    public class CapexAutoMapper : Profile
    {
        public CapexAutoMapper() 
        {
            CreateMap<CapexDTOPost, CapexRequest>();
            CreateMap<CapexRequest, CapexDTOGet>()
                              .ForMember(x => x.ApprovedBy, x => x.MapFrom(x => x.ApprovedByUser.FullName))
                              .ForMember(x => x.RequestedBy, x => x.MapFrom(x => x.RequestedByUser.FullName))
                              .ForMember(x=> x.RequiredRole, x=> x.MapFrom(x=>x.RequiredRole.RequiredRole.RoleName))
                              .ForMember(x => x.Approver, x=> x.MapFrom(x=>x.Approver.FullName));

            CreateMap<PRDTOPost2, PurchaseRequisition>();
            CreateMap<PurchaseRequisition, PRDTOGet2>()
                              .ForMember(x => x.ApprovedBy, x => x.MapFrom(x => x.ApprovedByUser.FullName))
                              .ForMember(x => x.RequestedBy, x => x.MapFrom(x => x.RequestedByUser.FullName))
                              .ForMember(x => x.RequiredRole, x => x.MapFrom(x => x.RequiredRole.RoleName));
        
            CreateMap<PRItemDTOPost2, PurchaseRequisitionItem>();
            CreateMap<PurchaseRequisitionItem, PRItemDTOGet2>();
        }


    }
}
