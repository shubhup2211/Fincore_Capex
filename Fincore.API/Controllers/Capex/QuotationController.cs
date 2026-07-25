using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        IQuotationService quotationService;

        public QuotationController(IQuotationService quotationService)
        {
            this.quotationService = quotationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuotation(int page = 1, int pagesize = 10)
        {
            var response = await quotationService.GetQuotation(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuotationById(int id)
        {
            var response = await quotationService.GetQuotationById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseQuotation(QuotationDTOPost quotation)
        {
            var response = await quotationService.CreateQuotation(quotation);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotationById(int id, QuotationDTOPost quotation)
        {
            var response = await quotationService.UpdateQuotation(id, quotation);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotationById(int id)
        {
            var response = await quotationService.DeleteQuotation(id);
            return Ok(response);
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitQuotation(int id)
        {
            var response = await quotationService.SubmitQuotation(id);
            return Ok(response);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetVendorQuotations(int vendorId)
        {
            var response = await quotationService.GetVendorQuotations(vendorId);
            return Ok(response);
        }

        [HttpGet("rfq/{rfqId}")]
        public async Task<IActionResult> GetRFQQuotations(int rfqId)
        {
            var response = await quotationService.GetRFQQuotations(rfqId);
            return Ok(response);
        }
    }
}