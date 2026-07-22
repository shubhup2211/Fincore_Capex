using Fincore.Application.DTO.MasterTable;

namespace Fincore.Application.Interfaces.IMasterTable
{
    public interface IDocumentService
    {
        Task<List<DocumentDto>> GetAll(int page,int pageSize);
        Task<DocumentDto> DocumentGetById(int id);
        Task<DocumentDto> AddDocument(CreateDocumentDto dto);
        Task<DocumentDto> UpdateDocument(int id,UpdateDocumentDto dto);

        Task<bool> DeleteDocument(int id);
    }

}