using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;

namespace Fincore.Application.Interfaces.IMasterTable
{
    public interface IDocumentService
    {
        Task<ApiResponse<List<DocumentDto>>> GetAll(int page, int pageSize);
        Task<ApiResponse<DocumentDto>> DocumentGetById(int id);
        Task<ApiResponse<DocumentDto>> AddDocument(CreateDocumentDto dto);
        Task<ApiResponse<DocumentDto>> UpdateDocument(int id,UpdateDocumentDto dto);

        Task<ApiResponse<bool>> DeleteDocument(int id);
    }

}