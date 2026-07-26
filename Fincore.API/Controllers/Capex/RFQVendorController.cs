using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Enums;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RFQVendorController : ControllerBase
    {
        IRFQVendorService rFQVendorService;

        public RFQVendorController(IRFQVendorService rFQVendorService)
        {
            this.rFQVendorService = rFQVendorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRFQVendor(int page = 1, int pagesize = 10, ResponseStatus? responseStatus=null)
        {
            var response = await rFQVendorService.GetRFQVendor(page, pagesize, responseStatus);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRFQVendorById(int id)
        {
            var response = await rFQVendorService.GetRFQVendorById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseRFQVendor(RFQVendorDTOPost rfqVendor)
        {
            var response = await rFQVendorService.CreateRFQVendor(rfqVendor);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRFQVendorById(int id, RFQVendorDTOPost rfqVendor)
        {
            var response = await rFQVendorService.UpdateRFQVendor(id, rfqVendor);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRFQVendorById(int id)
        {
            var response = await rFQVendorService.DeleteRFQVendor(id);
            return Ok(response);
        }

        [HttpGet("vendor/{vendorId}/submitted")]
        public async Task<IActionResult> GetSubmittedRFQForVendor(int vendorId)
        {
            var response = await rFQVendorService.GetSubmittedRFQForVendor(vendorId);
            return Ok(response);
        }

        [HttpGet("vendor/{vendorId}/pending")]
        public async Task<IActionResult> GetPendingRFQForVendor(int vendorId)
        {
            var response = await rFQVendorService.GetPendingRFQForVendor(vendorId);
            return Ok(response);
        }
    }
}