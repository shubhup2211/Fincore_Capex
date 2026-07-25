using Fincore.Application.Interfaces;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Services;
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
        public async Task<IActionResult> GetRevenue(int page = 1, int pageSize = 10)
        {
            var response = await service.GetRevenueAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("expense")]
        public async Task<IActionResult> GetExpense(int page = 1, int pageSize = 10)
        {
            var response = await service.GetExpenseAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("vendor-spend")]
        public async Task<IActionResult> GetVendorSpend(int page = 1, int pageSize = 10)
        {
            var response = await service.GetVendorSpendAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("capex")]
        public async Task<IActionResult> GetCapex(int page = 1, int pageSize = 10)
        {
            var response = await service.GetCapexAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("opex")]
        public async Task<IActionResult> GetOpex(int page = 1, int pageSize = 10)
        {
            var response = await service.GetOpexAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("budget-variance")]
        public async Task<IActionResult> GetBudgetVariance(int page = 1, int pageSize = 10)
        {
            var response = await service.GetBudgetVarianceAsync(page, pageSize);

            return Ok(response);
        }

        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLoss(int? companyId,int? departmentId,DateTime? fromDate,DateTime? toDate)
        {
            var response = await service.GetProfitLossAsync(companyId,departmentId,fromDate, toDate);

            return Ok(response);
        }

        [HttpGet("cash-flow")]
        public async Task<IActionResult> GetCashFlow(int? companyId, int? departmentId,DateTime? fromDate, DateTime? toDate)
        {
            var response = await service.GetCashFlowAsync(companyId,departmentId,fromDate, toDate);

            return Ok(response);
        }

        [HttpGet("balance-sheet")]
        public async Task<IActionResult> GetBalanceSheet(int page = 1,int pageSize = 10)
        {
            var response = await service.GetBalanceSheetAsync(page, pageSize);

            return Ok(response);
        }
    }
}