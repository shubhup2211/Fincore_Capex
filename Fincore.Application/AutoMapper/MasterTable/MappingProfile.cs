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


            // User Mapping
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role != null
                        ? src.Role.RoleName
                        : null));

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken,
                    opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Role,
                    opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken,
                    opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Role,
                    opt => opt.Ignore());


            // Permission Mapping
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role != null
                        ? src.Role.RoleName
                        : null))
                .ForMember(dest => dest.MasterTypeName,
                    opt => opt.MapFrom(src => src.MasterType != null
                        ? src.MasterType.MasterTypeName
                        : null));


            CreateMap<CreatePermissionDto, Permission>()
                .ForMember(dest => dest.PermissionId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(dest => dest.MasterType,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser,
                    opt => opt.Ignore());


            CreateMap<UpdatePermissionDto, Permission>()
                .ForMember(dest => dest.PermissionId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Role,
                    opt => opt.Ignore())
                .ForMember(dest => dest.MasterType,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser,
                    opt => opt.Ignore());


            // Role Mapping
            CreateMap<Role, RoleDto>();

            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.RoleId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Users,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Permissions,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalFlows,
                    opt => opt.Ignore());


            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.RoleId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUser,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Users,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Permissions,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalFlows,
                    opt => opt.Ignore());
        }
    }
}
