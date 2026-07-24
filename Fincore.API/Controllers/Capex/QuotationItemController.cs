using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuotationItemController : ControllerBase
    {
        IQuotationItemService quotationItemService;

        public QuotationItemController(IQuotationItemService quotationItemService)
        {
            this.quotationItemService = quotationItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuotationItem(int page = 1, int pagesize = 10)
        {
            var response = await quotationItemService.GetQuotationItem(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuotationItemById(int id)
        {
            var response = await quotationItemService.GetQuotationItemById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseQuotationItem(QuotationItemDTOPost quotationItem)
        {
            var response = await quotationItemService.CreateQuotationItem(quotationItem);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotationItemById(int id, QuotationItemDTOPost quotationItem)
        {
            var response = await quotationItemService.UpdateQuotationItem(id, quotationItem);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotationItemById(int id)
        {
            var response = await quotationItemService.DeleteQuotationItem(id);
            return Ok(response);
        }
    }
}