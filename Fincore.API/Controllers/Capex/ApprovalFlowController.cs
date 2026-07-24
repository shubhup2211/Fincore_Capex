using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApprovalFlowController : ControllerBase
    {
        IApprovalFlowService approvalFlowService;

        public ApprovalFlowController(IApprovalFlowService approvalFlowService)
        {
            this.approvalFlowService = approvalFlowService;
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovalFlow(int page = 1, int pagesize = 10)
        {
            var response = await approvalFlowService.GetApprovalFlow(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApprovalFlowById(int id)
        {
            var response = await approvalFlowService.GetApprovalFlowById(id);
            return Ok(response);
        }

        [HttpGet("amount/{amount}")]
        public async Task<IActionResult> GetApprovalFlowByAmount(decimal amount)
        {
            var response = await approvalFlowService.GetApprovalFlowByAmount(amount);
            return Ok(response);
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetApprovalFlowByRole(int roleId)
        {
            var response = await approvalFlowService.GetApprovalFlowByRole(roleId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApprovalFlow(ApprovalFlowDTOPost approvalFlow)
        {
            var response = await approvalFlowService.CreateApprovalFlow(approvalFlow);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApprovalFlow(int id, ApprovalFlowDTOPost approvalFlow)
        {
            var response = await approvalFlowService.UpdateApprovalFlow(id, approvalFlow);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApprovalFlow(int id)
        {
            var response = await approvalFlowService.DeleteApprovalFlow(id);
            return Ok(response);
        }
    }
}