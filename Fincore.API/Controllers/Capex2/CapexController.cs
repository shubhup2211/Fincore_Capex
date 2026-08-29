using Fincore.Application.DTO2;
using Fincore.Application.Interfaces.ICapex2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex2
{
    [Route("api/v1/my/[controller]")]
    [ApiController]
    public class CapexController : ControllerBase
    {
        ICapexService capexService;
        public CapexController(ICapexService capexService)
        {
            this.capexService = capexService;
        }

        [HttpPost]
        public async Task<IActionResult> raiseCapex(CapexDTOPost dto)
        {
            var response = await capexService.RaiseCapex(dto);
            return Ok(response);
        }

        [HttpGet("userCapex")]
        public async Task<IActionResult> getCapexOfUser(int page=1, int pageSize=10)
        {
            var response = await capexService.GetCapexByUserId(page, pageSize);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> getAllCapex(int page = 1, int pageSize = 10)
        {
            var response = await capexService.GetAllCapex(page, pageSize);
            return Ok(response);
        }

        [HttpGet("pendingApprovals")]
        public async Task<IActionResult> getPendingCapex(int page = 1, int pageSize = 10)
        {
            var response = await capexService.GetPendingCapex(page,pageSize);
            return Ok(response);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> approveCapex(int id)
        {
            var response = await capexService.ApproveCapex(id);
            return Ok(response);
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> rejectCapex(int id)
        {
            var response = await capexService.RejectCapex(id);
            return Ok(response);
        }

        [HttpGet("bugetLinesDept")]
        public async Task<IActionResult> bugetLinesDept()
        {
            var response = await capexService.GetBudgetLinesByDepartment();
            return Ok(response);
        }
    }
}
