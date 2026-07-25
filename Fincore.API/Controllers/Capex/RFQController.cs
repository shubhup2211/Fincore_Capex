using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RFQController : ControllerBase
    {
        IRFQService rFQService;

        public RFQController(IRFQService rFQService)
        {
            this.rFQService = rFQService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRFQ(int page = 1, int pagesize = 10)
        {
            var response = await rFQService.GetRFQ(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRFQById(int id)
        {
            var response = await rFQService.GetRFQById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseRFQ(RFQDTOPost rfq)
        {
            var response = await rFQService.CreateRFQ(rfq);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRFQById(int id, RFQDTOPost rfq)
        {
            var response = await rFQService.UpdateRFQ(id, rfq);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRFQById(int id)
        {
            var response = await rFQService.DeleteRFQ(id);
            return Ok(response);
        }

        [HttpPost("{id}/send")]
        public async Task<IActionResult> SendRFQ(int id)
        {
            var response = await rFQService.SendRFQ(id);
            return Ok(response);
        }

        [HttpGet("{id}/quotations")]
        public async Task<IActionResult> GetRFQQuotations(int id)
        {
            var response = await rFQService.GetRFQQuotations(id);
            return Ok(response);
        }

    }
}