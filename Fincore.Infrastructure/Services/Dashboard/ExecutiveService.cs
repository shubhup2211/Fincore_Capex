using Fincore.Application.DTO;
using Fincore.Application.DTO.Dashboard;
using Fincore.Application.Interfaces.Dashboard;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.Dashboard
{
    public class ExecutiveService : IExecutiveService
    {
         AppDbContext _db;

        public ExecutiveService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<ExecutiveDashboardDto>> GetExecutiveDashboard()
        {
            var dashboard = new ExecutiveDashboardDto
            {
                TotalRevenue = await _db.RevenueEntries.SumAsync(x => x.Amount),

                TotalInvoices = await _db.RevenueEntries.CountAsync(),

                ReceivedRevenue = await _db.RevenueEntries
                    .Where(x => x.Status == "Received")
                    .SumAsync(x => (decimal?)x.Amount) ?? 0,

                PendingRevenue = await _db.RevenueEntries
                    .Where(x => x.Status == "Pending")
                    .SumAsync(x => (decimal?)x.Amount) ?? 0,

                InvoiceCount = await _db.RevenueEntries
                    .Where(x => x.Status == "Invoiced")
                    .CountAsync(),

                ReceivedCount = await _db.RevenueEntries
                    .Where(x => x.Status == "Received")
                    .CountAsync(),

                PendingCount = await _db.RevenueEntries
                    .Where(x => x.Status == "Pending")
                    .CountAsync()
            };

            return ApiResponseHelper.SuccessRes(
                dashboard,
                "Dashboard fetched successfully"
            );
        }
    }
}