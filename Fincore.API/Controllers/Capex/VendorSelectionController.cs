using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VendorSelectionController : ControllerBase
    {
        IVendorSelectionService vendorSelectionService;

        public VendorSelectionController(IVendorSelectionService vendorSelectionService)
        {
            this.vendorSelectionService = vendorSelectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVendorSelection(int page = 1, int pagesize = 10)
        {
            var response = await vendorSelectionService.GetVendorSelection(page, pagesize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorSelectionById(int id)
        {
            var response = await vendorSelectionService.GetVendorSelectionById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseVendorSelection(VendorSelectionDTOPost vendorSelection)
        {
            var response = await vendorSelectionService.CreateVendorSelection(vendorSelection);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorSelectionById(int id, VendorSelectionDTOPost vendorSelection)
        {
            var response = await vendorSelectionService.UpdateVendorSelection(id, vendorSelection);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorSelectionById(int id)
        {
            var response = await vendorSelectionService.DeleteVendorSelection(id);
            return Ok(response);
        }

        [HttpGet("rfq/{rfqId}/comparison")]
        public async Task<IActionResult> CompareQuotations(int rfqId)
        {
            var response = await vendorSelectionService.CompareQuotations(rfqId);
            return Ok(response);
        }

        [HttpPost("rfq/{rfqId}/vendor/{vendorId}")]
        public async Task<IActionResult> SelectVendor(int rfqId, int vendorId)
        {
            var response = await vendorSelectionService.SelectVendor(rfqId, vendorId);
            return Ok(response);
        }

        [HttpGet("rfq/{rfqId}")]
        public async Task<IActionResult> GetSelectedVendorByRFQ(int rfqId)
        {
            var response = await vendorSelectionService.GetSelectedVendorByRFQ(rfqId);
            return Ok(response);
        }
    }
}