using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/[controller]")]
    [ApiController]
    public class PRItemController : ControllerBase
    {
        IPRItemService pRService;

        public PRItemController(IPRItemService pRService)
        {
            this.pRService = pRService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPRItem(int page = 1, int pagesize = 10)
        {
            var response = await pRService.GetPRItem(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseRequisitionItemById(int id)
        {
            var response = await pRService.GetPRItemById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddPRItem(PRItemDTOPost pr)
        {
            var response = await pRService.CreatePRItem(pr);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePRItemById(int id, PRItemDTOPost pr)
        {
            var response = await pRService.UpdatePRItem(id, pr);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePRItemById(int id)
        {
            var response = await pRService.DeletePRItem(id);
            return Ok(response);
        }
    }
}
