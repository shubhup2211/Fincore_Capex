using Fincore.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IExecutiveService _executiveService;

        public DashboardController(IExecutiveService executiveService)
        {
            _executiveService = executiveService;
        }

        [HttpGet("executive")]
        public async Task<IActionResult> GetExecutiveDashboard()
        {
            var result = await _executiveService.GetExecutiveDashboard();
            return Ok(result);
        }
    }
}