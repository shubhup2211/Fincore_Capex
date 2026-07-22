using AutoMapper;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NPOI.Util.Optional;

namespace Fincore.Infrastructure.Services.MasterTable
{
    public class DocumentService : IDocumentService
    {
         AppDbContext db;
        IMapper mapper;

        public DocumentService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<DocumentDto>> GetAll(int page,int pageSize)
        {
            var data= await db.Documents
                .Skip((page -1) *pageSize)
                .Take(pageSize)
                .ToListAsync();

            return mapper.Map<List<DocumentDto>>(data);
        }

        public async Task<DocumentDto> DocumentGetById(int id)
        {


            var data = await db.Documents
                .FirstOrDefaultAsync(x=>x.DocumentsId==id);
            return mapper.Map<DocumentDto>(data);


            //var data = await db.Documents
            //    .Where(x => x.DocumentsId == id)
            //    .Select(x => new DocumentDto
            //    {
            //        DocumentsId = x.DocumentsId,
            //        DocumentTypeId = x.DocumentTypeId,
            //        UserId = x.UserId,
            //        EntityId = x.EntityId,
            //        MasterTypeId = x.MasterTypeId,
            //        FileName = x.FileName,
            //        FileType = x.FileType,
            //        FilePath = x.FilePath,
            //        CreatedAt = x.CreatedAt,
            //        ModifiedAt = x.ModifiedAt
            //    })
            //    .FirstOrDefaultAsync();

            //return data;
        }


        public async Task<DocumentDto> AddDocument(CreateDocumentDto dto) 
        {
            var data = mapper.Map<Document>(dto);
            data.CreatedAt = DateTime.Now;
            data.ModifiedAt=DateTime.Now;
            db.Documents.Add(data);
            await db.SaveChangesAsync();
            return mapper.Map<DocumentDto>(data);
        }

       

        public async Task<DocumentDto> UpdateDocument(int id ,UpdateDocumentDto dto)
        {
            var data = await db.Documents
                .FirstOrDefaultAsync(x => x.DocumentsId == id);
            if (data == null)
                return null;
            mapper.Map(dto, data);
            await db.SaveChangesAsync();
            return mapper.Map<DocumentDto>(data);
        }

        public async Task<bool> DeleteDocument(int id)
        {
            var data = await db.Documents
                .FirstOrDefaultAsync(x => x.DocumentsId == id);
            if (data == null) 
            {
                return false;
            }
            db.Documents.Remove(data);
            await db.SaveChangesAsync();
            return true;
        }
    }


}