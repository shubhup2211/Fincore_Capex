using AutoMapper;
using Fincore.Application.DTO.MasterTable;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.AutoMapper.MasterTable
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
    
            CreateMap<CreateCompanyDto, Company>();

           
            CreateMap<UpdateCompanyDto, Company>();

            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.CountryName,
                    opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : ""))

                .ForMember(dest => dest.MasterTypeName,
                    opt => opt.MapFrom(src => src.MasterType != null ? src.MasterType.MasterTypeName : ""));

            CreateMap<Department, DepartmentDTO>()
    .ForMember(dest => dest.CompanyName,
        opt => opt.MapFrom(src => src.Company.CompanyName))

    .ForMember(dest => dest.MasterTypeName,
        opt => opt.MapFrom(src => src.MasterType != null
            ? src.MasterType.MasterTypeName
            : null))

    .ForMember(dest => dest.ManagerName,
        opt => opt.MapFrom(src => src.Manager != null
            ? src.Manager.User.FullName
            : null));

            CreateMap<DepartmentDTO, Department>();
        }
    }
}
