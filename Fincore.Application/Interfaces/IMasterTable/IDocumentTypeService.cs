using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.IMasterTable
{
    public interface IDocumentTypeService
    {
        Task<ApiResponse<List<DocumentTypeDto>>> GetAllDocumentType(int page, int pageSize);
        Task<ApiResponse<DocumentTypeDto>> GetByIdDocumentType(int id);
        Task<ApiResponse<DocumentTypeDto>> AddDocumentType(CreateDocumentTypeDto dto);
        Task<ApiResponse<DocumentTypeDto>> UpdateDocumentType(int id,UpdateDocumentTypeDto dto);
        Task<ApiResponse<bool>> DeleteDocumentType(int id);
    }
}
