using Fincore.Application.DTO.Payment.RevenueEntry.Requests;
using Fincore.Application.Interfaces.IPayment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FINCORE.API.Controllers.V1
{
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    [Route("api/v1/revenue")]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService revenueService;

        public RevenueController(IRevenueService revenueService)
        {
            this.revenueService = revenueService;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRevenueEntryRequestDto request)
        {
            var result = await revenueService.CreateAsync(request);
            return Ok(result);
        }

        // Get All
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await revenueService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        // Get By Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await revenueService.GetByIdAsync(id);
            return Ok(result);
        }

        // Update
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateRevenueEntryRequestDto request)
        {
            var result = await revenueService.UpdateAsync(id, request);
            return Ok(result);
        }

        // Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await revenueService.DeleteAsync(id);
            return Ok(result);
        }

        //// Approve
        //[HttpPut("{id:int}/approve")]
        //public async Task<IActionResult> Approve(int id)
        //{
        //    var result = await revenueService.ApproveAsync(id);
        //    return Ok(result);
        //}

        //// Reject
        //[HttpPut("{id:int}/reject")]
        //public async Task<IActionResult> Reject(int id)
        //{
        //    var result = await revenueService.RejectAsync(id);
        //    return Ok(result);
        //}


        [HttpPut("{id:int}/invoice")]
        public async Task<IActionResult> MarkAsInvoiced(int id)
        {
            var result = await revenueService.MarkAsInvoicedAsync(id);
            return Ok(result);
        }



        [HttpPut("{id:int}/receive")]
        public async Task<IActionResult> MarkAsReceived(int id)
        {
            var result = await revenueService.MarkAsReceivedAsync(id);
            return Ok(result);
        }





        // Filter By Status
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await revenueService.GetRevenueByStatusAsync(status, page, pageSize);
            return Ok(result);
        }

        // Filter By Type
        [HttpGet("type/{revenueType}")]
        public async Task<IActionResult> GetByType(
            string revenueType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await revenueService.GetRevenueByTypeAsync(revenueType, page, pageSize);
            return Ok(result);
        }

        // Monthly Report
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var result = await revenueService.GetMonthlyRevenueAsync();
            return Ok(result);
        }

        // Dashboard Summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var result = await revenueService.GetRevenueSummaryAsync();
            return Ok(result);
        }
    }
}