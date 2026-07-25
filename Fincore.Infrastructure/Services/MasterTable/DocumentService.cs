using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NPOI.Util.Optional;
using Fincore.Infrastructure.CommonHelper;
using Azure.Core;


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

        public async Task<ApiResponse<List<DocumentDto>>> GetAll(int page, int pageSize)
        {
            var totalRecords = await db.Documents.CountAsync();

            var data = await db.Documents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<DocumentDto>>(
                    "Document Not Found",
                    "404",
                    "Invalid Document"
                );
            }

            var res = mapper.Map<List<DocumentDto>>(data);

            return ApiResponseHelper.SuccessRes(
                res,
                "Documents Fetch Successfully",
                totalRecords,
                new { page=page, pageSize=pageSize}
            );
        }

        public async Task<ApiResponse<DocumentDto>> DocumentGetById(int id)
        {


            var data = await db.Documents
                .FirstOrDefaultAsync(x=>x.DocumentsId==id);
            if (data == null) 
            {
                return ApiResponseHelper.Failure<DocumentDto>
                    (
                    "Document Not Found",
                    "404",
                    "Invalid Dcument Id"
                    
                    );
            }   
            DocumentDto res= mapper.Map<DocumentDto>(data);

            return ApiResponseHelper.SuccessRes(res,"Document Get Successfully");


        
        }


        public async Task<ApiResponse<DocumentDto>> AddDocument(CreateDocumentDto dto)
        {
            var data = mapper.Map<Document>(dto);

            data.CreatedAt = DateTime.Now;
            data.ModifiedAt = DateTime.Now;

            db.Documents.Add(data);
            await db.SaveChangesAsync();

            var result = mapper.Map<DocumentDto>(data);

            return ApiResponseHelper.SuccessRes(
                result,
                "Document Added Successfully"
            );
        }



        public async Task<ApiResponse<DocumentDto>> UpdateDocument(int id, UpdateDocumentDto dto)
        {
            var data = await db.Documents
                .FirstOrDefaultAsync(x => x.DocumentsId == id);

            if (data == null)
            {
                return ApiResponseHelper.Failure<DocumentDto>(
                    "Document Not Found",
                    "404",
                    "Invalid Document Id"
                );
            }

            mapper.Map(dto, data);
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            var result = mapper.Map<DocumentDto>(data);

            return ApiResponseHelper.SuccessRes(
                result,
                "Document Updated Successfully"
            );
        }

        public async Task<ApiResponse<bool>> DeleteDocument(int id)
        {
            var data = await db.Documents
                .FirstOrDefaultAsync(x => x.DocumentsId == id);

            if (data == null)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Document Not Found",
                    "404",
                    "Invalid Document Id"
                );
            }

            db.Documents.Remove(data);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes(
                true,
                "Document Deleted Successfully"
            );
        }
    }


}