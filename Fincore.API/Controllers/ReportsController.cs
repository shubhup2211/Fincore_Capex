using Fincore.Application.Interfaces;
using Fincore.Infrastructure.CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService service;

        public ReportsController(IReportService service)
        {
            this.service = service;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var result = await service.GetRevenueAsync();

            return Ok(ApiResponseHelper.SuccessRes(result, "Revenue report fetched successfully.", result.Count
             ));

        }

        [HttpGet("expense")]
        public async Task<IActionResult> GetExpense()
        {
            var result = await service.GetExpenseAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "Expense report fetched successfully.",
                result.Count
            ));
        }

        [HttpGet("vendor-spend")]
        public async Task<IActionResult> GetVendorSpend()
        {
            var result = await service.GetVendorSpendAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "Vendor spend report fetched successfully.",
                result.Count
            ));
        }


        [HttpGet("capex")]
        public async Task<IActionResult> GetCapex()
        {
            var result = await service.GetCapexAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "CAPEX report fetched successfully.",
                result.Count
            ));
        }

        [HttpGet("opex")]
        public async Task<IActionResult> GetOpex()
        {
            var result = await service.GetOpexAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "OPEX report fetched successfully.",
                result.Count
            ));
        }

        [HttpGet("budget-variance")]
        public async Task<IActionResult> GetBudgetVariance()
        {
            var result = await service.GetBudgetVarianceAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "Budget variance report fetched successfully.",
                result.Count
            ));
        }

        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLoss()
        {
            var result = await service.GetProfitLossAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "Profit & Loss report fetched successfully.",
                1
            ));
        }

        [HttpGet("cash-flow")]
        public async Task<IActionResult> GetCashFlow()
        {
            var result = await service.GetCashFlowAsync();

            return Ok(ApiResponseHelper.SuccessRes(
                result,
                "Cash flow report fetched successfully.",
                1
            ));
        }
    }
}
