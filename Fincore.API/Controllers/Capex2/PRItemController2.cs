using Fincore.Application.DTO2;
using Fincore.Application.Interfaces.ICapex2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Capex2
{
    [Route("api/v1/my/[controller]")]
    [ApiController]
    public class PRItemController2 : ControllerBase
    {
        private readonly IPRItemService2 prItemService;
        public PRItemController2(IPRItemService2 prItemService) 
        {
            this.prItemService = prItemService;
        }

        [HttpGet("getPRItems/{prId}")]
        public async Task<IActionResult> getPRItem(int prId)
        { 
          var response = await prItemService.getPRItemByPR(prId);
            return Ok(response);
        }

        [HttpPost("addPRItem")]
        public async Task<IActionResult> addPRItem(PRItemDTOPost2 dto)
        {
            var response = await prItemService.addPRItem(dto);
            return Ok(response);
        }
        [HttpPost("deletePRItem")]
        public async Task<IActionResult> deletePRItem(int prItem)
        {
            var response = await prItemService.deletePRItem(prItem);
            return Ok(response);
        }
    }
}
