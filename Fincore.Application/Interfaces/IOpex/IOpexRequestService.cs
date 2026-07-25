using Fincore.Application.DTO;
using Fincore.Application.DTOs.OpexRequest;

namespace Fincore.Application.Interfaces.Opex
{
    public interface IOpexRequestService
    {
        Task AddOpexRequest(CreateOpexRequestDTO dto);

        Task<List<OpexRequestResponseDTO>> GetOpexRequests(int page, int pageSize);

        Task<OpexRequestResponseDTO?> GetOpexRequestById(int id);

        Task UpdateOpexRequest(int id, UpdateOpexRequestDTO dto);

        Task DeleteOpexRequest(int id);

        Task<string> ApproveOpexRequest(int id, int approvedBy);

        Task<string> RejectOpexRequest(int id, int approvedBy);
          Task<OpexSummaryDTO> GetOpexSummary();
    }
}