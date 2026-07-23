using AutoMapper;
using Fincore.Application.DTO.MasterTable;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Fincore.Domain.Models;

namespace Fincore.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Document, DocumentDto>();
            CreateMap<CreateDocumentDto, Document>();
            CreateMap<UpdateDocumentDto, Document>();


            CreateMap<DocumentType, DocumentTypeDto>();
            CreateMap<CreateDocumentTypeDto, DocumentType>();
            CreateMap<UpdateDocumentTypeDto, DocumentType>();


            CreateMap<MasterType, MasterTypeDto>();
            CreateMap<CreateMasterTypeDto, MasterType>();
            CreateMap<UpdateMasterTypeDto, MasterType>();



        }




    }
}
