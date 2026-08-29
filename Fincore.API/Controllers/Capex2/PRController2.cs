using Fincore.Application.DTO2;
using Fincore.Application.Interfaces.ICapex2;
using Fincore.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex2
{
    [Route("api/v1/my/[controller]")]
    [ApiController]
    public class PRController2 : ControllerBase
    {
        private readonly IPRService2 prService;
        public PRController2(IPRService2 prService)
        {
            this.prService = prService;
        }

        [HttpPost("raisePRequest")]
        public async Task<IActionResult> raisePR(PRDTOPost2 dto)
        {
            var response = await prService.raisePR(dto);
            return Ok(response);
        }

        [HttpGet("allPR")]
        public async Task<IActionResult> getallPR(int page=1, int pageSize=10, IsActive? status=null, string? search=null)
        {
            var response = await prService.getAllPR(page, pageSize, status,search);
            return Ok(response);
        }

        [HttpGet("pendingPR")]
        public async Task<IActionResult> pendingPR(int page=1, int pageSize=10, string? searc = null)
        {
            var response = await prService.getPendingPR(page, pageSize, searc);
            return Ok(response);
        }

        [HttpPost("approvePR/{id}")]
        public async Task<IActionResult> approvePR(int pid)
        {
            var response = await prService.approvePR(pid);
            return Ok(response);
        }

        [HttpPost("rejectPR/{id}")]
        public async Task<IActionResult> rejectPR(int id)
        {
            var response = await prService.rejectPR(id);
            return Ok(response);
        }

        [HttpPost("submitPR/{id}")]
        public async Task<IActionResult> submitPR(int id)
        {
            var response = await prService.submitPR(id);
            return Ok(response);
        }
    }
}
