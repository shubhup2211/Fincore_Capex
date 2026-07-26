using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IApprovalFlowService
    {
        Task<ApiResponse<string>> CreateApprovalFlow(ApprovalFlowDTOPost approvalFlow);
        Task<ApiResponse<string>> UpdateApprovalFlow(int id, ApprovalFlowDTOPost approvalFlow);
        Task<ApiResponse<string>> DeleteApprovalFlow(int id);
        Task<ApiResponse<ApprovalFlowDTOGet>> GetApprovalFlowById(int id);
        Task<ApiResponse<List<ApprovalFlowDTOGet>>> GetApprovalFlow(int page, int pagesize, IsActive? isActive);
        Task<ApiResponse<ApprovalFlowDTOGet>> GetApprovalFlowByAmount(decimal amount);
        Task<ApiResponse<List<ApprovalFlowDTOGet>>> GetApprovalFlowByRole(int roleId);
    }
}
