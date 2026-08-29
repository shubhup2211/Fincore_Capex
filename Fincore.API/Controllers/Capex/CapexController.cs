using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CapexController : ControllerBase
    {
        ICapexReq capex;

        public CapexController(ICapexReq capex) 
        {
            this.capex = capex;
        }

        [HttpPost]
        public async Task<IActionResult> RaiseCapex(CapexReqDTOPost cap) 
        {
           var response = await capex.RaiseCapex(cap);
            return Ok(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCapex(int page=1, int pagesize=5)
        {
            var response = await capex.GetCapex(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCapexById(int id)
        {
            var response = await capex.GetCapexById(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCapexById(int id, CapexReqDTOPost dto)
        {
            var response = await capex.UpdateCapex(id, dto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCapexById(int id)
        {
            var resonse = await capex.DeleteCapex(id);
            return Ok(resonse);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitCapex(CapexReqDTOPost dto)
        {
            var userId = CurrentUser.GetUserId(User);

            var response = await capex.SubmitCapex(dto,userId);
            return Ok(response);
        }


        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveCapex(int id)
        {
            var userId = CurrentUser.GetUserId(User);

            var response = await capex.ApproveCapex(id,userId);
            return Ok(response);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectCapex(int id)
        {
            var userId = CurrentUser.GetUserId(User);

            var response = await capex.RejectCapex(id, userId);
            return Ok(response);
        }

        [HttpGet("PendingApproval")]
        public async Task<IActionResult> PendingApproval()
        {
            var userId = CurrentUser.GetUserId(User);
            var result = await capex.GetPendingApprovals(userId);
            return Ok(result);
        }


        //testing
        [HttpPost("submit-test")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitCapex(CapexReqDTOPost dto, int userId)
        {
            var response = await capex.SubmitCapex(dto, userId);
            return Ok(response);
        }

        [HttpGet("PendingApproval-test")]
        [AllowAnonymous]
        public async Task<IActionResult> PendingApproval(int userId)
        {
            var result = await capex.GetPendingApprovals(userId);
            return Ok(result);
        }

        [HttpPost("{id}/approve-test")]
        [AllowAnonymous]
        public async Task<IActionResult> ApproveCapex(int id, int userId)
        {
            var response = await capex.ApproveCapex(id, userId);
            return Ok(response);
        }

        [HttpPost("{id}/reject-test")]
        [AllowAnonymous]
        public async Task<IActionResult> RejectCapex(int id, int userId)
        {
            var response = await capex.RejectCapex(id, userId);
            return Ok(response);
        }
    }
}
