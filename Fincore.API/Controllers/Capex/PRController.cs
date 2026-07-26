using Fincore.Application.Constants;
using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Enums;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PRController : ControllerBase
    {
        IPRService pRService;

        public PRController(IPRService pRService)
        {
            this.pRService = pRService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseRequisition(int page=1,int pagesize=10, IsActive? status=null, Domain.Enums.ApprovalStatus? approvalStatus = null) 
        {
            var response = await pRService.GetPR(page,pagesize,status,approvalStatus);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseRequisitionById(int id)
        {
            var response = await pRService.GetPRById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaisePR (PRDTOPost pr)
        {
            var response = await pRService.CreatePR(pr);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePRById(int id, PRDTOPost pr)
        {
            var response = await pRService.UpdatePR(id,pr);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePRById(int id)
        {
            var response = await pRService.DeletePR(id);
            return Ok(response);
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitPR(int id)
        {
            var response = await pRService.SubmitPR(id);
            return Ok(response);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePR(int id)
        {
            var response = await pRService.ApprovePR(id);
            return Ok(response);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectPR(int id)
        {
            var response = await pRService.RejectPR(id);
            return Ok(response);
        }
    }
}
