using Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/v1/general-ledger")]
    public class GeneralLedgerController : ControllerBase
    {
        private readonly IGeneralLedgerService service;

        public GeneralLedgerController(IGeneralLedgerService service)
        {
            this.service = service;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var result = await service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await service.GetByIdAsync(id);
            return Ok(result);
        }

        
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await service.GetSummaryAsync();
            return Ok(result);
        }

        [HttpGet("trial-balance")]
        public async Task<IActionResult> GetTrialBalance()
        {
            var result = await service.GetTrialBalanceAsync();
            return Ok(result);
        }

        [HttpGet("trial-balance/summary")]
        public async Task<IActionResult> GetTrialBalanceSummary()
        {
            var result = await service.GetTrialBalanceSummaryAsync();
            return Ok(result);
        }

        [HttpGet("accounts/{accountId}")]
        public async Task<IActionResult> GetLedgerAccount( int accountId,int page = 1,int pageSize = 10)
        {
            var result = await service.GetLedgerAccountAsync(accountId,page,pageSize);

            return Ok(result);
        }

        [HttpGet("accounting-reports")]
        public async Task<IActionResult> GetAccountingReport( DateTime? fromDate, DateTime? toDate,int? accountId, int page = 1,int pageSize = 10)
        {
            var result = await service.GetAccountingReportAsync(fromDate,toDate, accountId, page,pageSize);

            return Ok(result);
        }
    }
}
