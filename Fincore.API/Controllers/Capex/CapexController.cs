using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
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

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitCapex(int id)
        {
            var response = await capex.SubmitCapex(id);
            return Ok(response);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveCapex(int id)
        {
            var response = await capex.ApproveCapex(id);
            return Ok(response);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectCapex(int id)
        {
            var response = await capex.RejectCapex(id);
            return Ok(response);
        }
    }
}
