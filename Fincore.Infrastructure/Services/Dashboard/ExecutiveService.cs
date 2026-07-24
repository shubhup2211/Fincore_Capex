using Fincore.Application.DTO;
using Fincore.Application.DTO.Dashboard;
using Fincore.Application.Interfaces.Dashboard;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Fincore.Infrastructure.Services.Dashboard
{
    public class ExecutiveService : IExecutiveService
    {
         AppDbContext db;
        IMemoryCache cache;
        public ExecutiveService(AppDbContext db,IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<ApiResponse<ExecutiveDashboardDto>> GetExecutiveDashboard()
        {
            const string cacheKey = "ExecutiveDashboard";

            if (!cache.TryGetValue(cacheKey, out ExecutiveDashboardDto dashboard))
            {
                dashboard = new ExecutiveDashboardDto
                {
                    TotalRevenue = await db.RevenueEntries.SumAsync(x => x.Amount),

                    TotalInvoices = await db.RevenueEntries.CountAsync(),

                    ReceivedRevenue = await db.RevenueEntries
                        .Where(x => x.Status == "Received")
                        .SumAsync(x => (decimal?)x.Amount) ?? 0,

                    PendingRevenue = await db.RevenueEntries
                        .Where(x => x.Status == "Pending")
                        .SumAsync(x => (decimal?)x.Amount) ?? 0,

                    InvoiceCount = await db.RevenueEntries
                        .Where(x => x.Status == "Invoiced")
                        .CountAsync(),

                    ReceivedCount = await db.RevenueEntries
                        .Where(x => x.Status == "Received")
                        .CountAsync(),

                    PendingCount = await db.RevenueEntries
                        .Where(x => x.Status == "Pending")
                        .CountAsync()
                };

                cache.Set(cacheKey, dashboard, TimeSpan.FromMinutes(5));
            }

            return ApiResponseHelper.SuccessRes(
                dashboard,
                "Dashboard fetched successfully");
        }
    }
}