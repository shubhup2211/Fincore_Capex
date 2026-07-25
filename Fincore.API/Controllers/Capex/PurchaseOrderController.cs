using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class PurchaseOrderController : ControllerBase
    {

        private readonly IPurchaseOrderService service;


        public PurchaseOrderController(IPurchaseOrderService service)
        {
            this.service = service;
        }


        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder(PurchaseOrderDTO dto)
        {
            return Ok(await service.AddPurchaseOrder(dto));
        }



        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrder(
            int page = 1,
            int pageSize = 10)
        {
            return Ok(await service.GetAllPurchaseOrder(page, pageSize));
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            return Ok(await service.GetPurchaseOrder(id));
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(
            int id,
            PurchaseOrderDTO dto)
        {
            return Ok(await service.UpdatePurchaseOrder(id, dto));
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            return Ok(await service.DeletePurchaseOrder(id));
        }



        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePurchaseOrder(int id)
        {
            return Ok(await service.ApprovePurchaseOrder(id));
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPurchaseOrderByStatus(
                    string status,
                    int page = 1,
                    int pageSize = 10)
        {
            return Ok(await service.GetPurchaseOrderByStatus(status, page, pageSize));
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPurchaseOrder(int id)
        {
            return Ok(await service.CancelPurchaseOrder(id));
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> ClosePurchaseOrder(int id)
        {
            return Ok(await service.ClosePurchaseOrder(id));
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GeneratePdf(int id)
        {

            var pdf = await service.GeneratePurchaseOrderPdf(id);


            if (pdf == null)
            {
                return NotFound(
                    "Purchase Order Not Found"
                );
            }


            return File(
                pdf,
                "application/pdf",
                $"PurchaseOrder_{id}.pdf"
            );

        }



        [HttpGet("filter")]
        public async Task<IActionResult> FilterPurchaseOrder([FromQuery] PurchaseOrderFilterDTO filter)
        {
            return Ok(
                await service.FilterPurchaseOrders(filter)
            );
        }

        
    }
}