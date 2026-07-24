using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO.MasterTable;
using Fincore.Domain.Models;

namespace Fincore.Application.AutoMapper.MasterTable
{
    public class AccountsMasterMapper : Profile
    {
        public AccountsMasterMapper()
        {
            CreateMap<AccountMaster, AccountMasterPostDTO>().ReverseMap();
            CreateMap<AccountMaster, AccountMasterGetDTO>().ReverseMap();

            CreateMap<AccountMaster, AccountMasterPutDTO>().ReverseMap();
        }
    }
}
