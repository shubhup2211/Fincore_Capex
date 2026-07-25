using Fincore.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fincore.API.Controllers.Dashboard
{
    [Route("api/[Action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        IExecutiveService erepo;
        IFinanceService frepo;
        IProcurementService prepo;
        IBudgetService brepo;

        public DashboardController(IExecutiveService erepo,IFinanceService frepo,IProcurementService prepo,IBudgetService brepo)
        {
            this.erepo = erepo;
            this.frepo = frepo;
            this.prepo = prepo;
            this.brepo = brepo;
            
        }

        [HttpGet]
        public async Task<IActionResult> GetExecutiveDashboard()
        {
            var result = await erepo.GetExecutiveDashboard();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFinanceDashboard() 
        {
            var res = await frepo.GetFinanceDashboard();
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetProcurementDashboard() 
        {
            var res = await prepo.GetProcurementDashboard();
            return Ok(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetBudgetDashboard()
        {
            var res=await brepo.GetBudgetDashboard();
            return Ok(res);
        }
    }
}