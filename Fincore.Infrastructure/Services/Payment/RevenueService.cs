using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment.RevenueEntry.Requests;
using Fincore.Application.DTO.Payment.RevenueEntry.Responses;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services.PaymentModule
{
    public class RevenueService : IRevenueService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        private const string RevenueCacheKey = "RevenueEntries";

        public RevenueService(
            AppDbContext db,
            IMapper mapper,
            IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }

        public async Task<ApiResponse<RevenueEntryResponseDto>> CreateAsync(CreateRevenueEntryRequestDto request)
        {

            bool customerExists = await db.Customers
                    .AnyAsync(x => x.CustomerId == request.CustomerId);

            if (!customerExists)
                throw new Exception("Customer not found.");
            // Validate Customer
            var customer = await db.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId);

            if (customer == null)
                throw new Exception("Customer not found.");

            // Validate Department
            var department = await db.Departments
                .FirstOrDefaultAsync(x => x.DepartmentId == request.DepartmentId);

            if (department == null)
                throw new Exception("Department not found.");

            // Validate Account
            var account = await db.AccountMasters
                .FirstOrDefaultAsync(x => x.AccountId == request.AccountId);

            if (account == null)
                throw new Exception("Account not found.");

            // Generate Revenue Invoice Number
            var lastRevenue = await db.RevenueEntries
                .OrderByDescending(x => x.RevenueEntryId)
                .FirstOrDefaultAsync();

            int nextId = (lastRevenue?.RevenueEntryId ?? 0) + 1;

            string invoiceNumber = $"REV-{DateTime.Now.Year}-{nextId:D5}";

            // Map DTO to Entity
            var revenueEntry = mapper.Map<RevenueEntry>(request);

            revenueEntry.InvoiceNumber = invoiceNumber;
            revenueEntry.Status = "Pending";
            revenueEntry.CreatedAt = DateTime.Now;
            revenueEntry.ModifiedAt = DateTime.Now;

            // TODO:
            // Get UserId from JWT Claims once Authentication is completed
            // revenueEntry.CreatedBy = loggedInUserId;
            // revenueEntry.ModifiedBy = loggedInUserId;
            revenueEntry.CreatedBy = 1;
            revenueEntry.ModifiedBy = 1;

            await db.RevenueEntries.AddAsync(revenueEntry);
            await db.SaveChangesAsync();

            // Clear Cache
            cache.Remove(RevenueCacheKey);

            // Fetch Newly Created Record
            var result = await db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntry.RevenueEntryId);

            var response = mapper.Map<RevenueEntryResponseDto>(result);

            return ApiResponseHelper.SuccessRes(
                response,
                "Revenue Entry Created Successfully."
            );
        }

        public async Task<ApiResponse<List<RevenueEntryResponseDto>>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            string cacheKey = $"RevenueEntries_Page_{page}_Size_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<RevenueEntryResponseDto> cachedData))
            {
                int cachedTotal = await db.RevenueEntries.CountAsync();

                return ApiResponseHelper.SuccessRes(
                    cachedData,
                    "Revenue Entries fetched successfully.",
                    cachedTotal);
            }

            var query = db.RevenueEntries
                 .Where(x => x.Status != "Deleted")
                 .AsNoTracking()
                 .Include(x => x.Customer)
                 .Include(x => x.Department)
                 .Include(x => x.AccountMaster);

            int totalRecords = await query.CountAsync();

            var revenueEntries = await query
                .OrderBy(x => x.RevenueEntryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = mapper.Map<List<RevenueEntryResponseDto>>(revenueEntries);

            cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                result,
                "Revenue Entries fetched successfully.",
                totalRecords);
        }

        public async Task<ApiResponse<RevenueEntryResponseDto>> GetByIdAsync(int revenueEntryId)
        {
            string cacheKey = $"RevenueEntry_{revenueEntryId}";

            if (cache.TryGetValue(cacheKey, out RevenueEntryResponseDto cachedRevenue))
            {
                return ApiResponseHelper.SuccessRes(
                    cachedRevenue,
                    $"Revenue Entry fetched successfully.");
            }

            var revenue = await db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .FirstOrDefaultAsync(x =>
                                        x.RevenueEntryId == revenueEntryId &&
                                        x.Status != "Deleted");

            if (revenue == null)
                throw new Exception("Revenue Entry not found.");

            var result = mapper.Map<RevenueEntryResponseDto>(revenue);

            cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                result,
                "Revenue Entry fetched successfully.");
        }

        public async Task<ApiResponse<RevenueEntryResponseDto>> UpdateAsync(
    int revenueEntryId,
    UpdateRevenueEntryRequestDto request)
        {
            // Check Revenue Entry
            var revenueEntry = await db.RevenueEntries
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (revenueEntry == null)
                throw new Exception("Revenue Entry not found.");

            // Revenue cannot be updated once Invoiced
            if (revenueEntry.Status == "Invoiced")
                throw new Exception("Invoiced Revenue Entry cannot be updated.");

            // Revenue cannot be updated once Payment is Received
            if (revenueEntry.Status == "Received")
                throw new Exception("Received Revenue Entry cannot be updated.");

            // Validate Customer
            bool customerExists = await db.Customers
                .AnyAsync(x => x.CustomerId == request.CustomerId);

            if (!customerExists)
                throw new Exception("Customer not found.");

            // Validate Department
            bool departmentExists = await db.Departments
                .AnyAsync(x => x.DepartmentId == request.DepartmentId);

            if (!departmentExists)
                throw new Exception("Department not found.");

            // Validate Account
            bool accountExists = await db.AccountMasters
                .AnyAsync(x => x.AccountId == request.AccountId);

            if (!accountExists)
                throw new Exception("Account not found.");

            // Update Editable Fields
            revenueEntry.CustomerId = request.CustomerId;
            revenueEntry.DepartmentId = request.DepartmentId;
            revenueEntry.RevenueType = request.RevenueType;
            revenueEntry.Amount = request.Amount;
            revenueEntry.RevenueDate = request.RevenueDate;
            revenueEntry.AccountId = request.AccountId;

            // Do NOT Update:
            // InvoiceNumber
            // Status
            // CreatedAt
            // CreatedBy

            revenueEntry.ModifiedAt = DateTime.Now;

            // TODO:
            // Get UserId from JWT Claims
            // revenueEntry.ModifiedBy = loggedInUserId;

            await db.SaveChangesAsync();

            // Clear Cache
            cache.Remove(RevenueCacheKey);
            cache.Remove($"RevenueEntry_{revenueEntryId}");

            // Fetch Updated Record
            var updatedRevenue = await db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (updatedRevenue == null)
                throw new Exception("Revenue Entry not found after update.");

            var response = mapper.Map<RevenueEntryResponseDto>(updatedRevenue);

            return ApiResponseHelper.SuccessRes(
                response,
                "Revenue Entry updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int revenueEntryId)
        {
            // Check Revenue Entry
            var revenueEntry = await db.RevenueEntries
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (revenueEntry == null)
                throw new Exception("Revenue Entry not found.");

            // Cannot delete once invoiced
            if (revenueEntry.Status == "Invoiced")
                throw new Exception("Invoiced Revenue Entry cannot be deleted.");

            // Cannot delete once payment is received
            if (revenueEntry.Status == "Received")
                throw new Exception("Received Revenue Entry cannot be deleted.");

            // Already deleted
            if (revenueEntry.Status == "Deleted")
                throw new Exception("Revenue Entry is already deleted.");

            // Soft Delete
            revenueEntry.Status = "Deleted";
            revenueEntry.ModifiedAt = DateTime.Now;

            // TODO:
            // Get UserId from JWT Claims
            // revenueEntry.ModifiedBy = loggedInUserId;

            await db.SaveChangesAsync();

            // Clear Cache
            cache.Remove(RevenueCacheKey);
            cache.Remove($"RevenueEntry_{revenueEntryId}");

            return ApiResponseHelper.SuccessRes(
                "Revenue Entry deleted successfully.");
        }

        public async Task<ApiResponse<RevenueEntryResponseDto>> MarkAsInvoicedAsync(int revenueEntryId)
        {
            // Check Revenue Entry
            var revenueEntry = await db.RevenueEntries
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (revenueEntry == null)
                throw new Exception("Revenue Entry not found.");

            // Already Invoiced
            if (revenueEntry.Status == "Invoiced")
                throw new Exception("Revenue Entry is already invoiced.");

            // Payment already received
            if (revenueEntry.Status == "Received")
                throw new Exception("Received Revenue Entry cannot be invoiced again.");

            // Deleted
            if (revenueEntry.Status == "Deleted")
                throw new Exception("Deleted Revenue Entry cannot be invoiced.");

            // Only Pending can be Invoiced
            if (revenueEntry.Status != "Pending")
                throw new Exception("Only Pending Revenue Entry can be invoiced.");

            revenueEntry.Status = "Invoiced";
            revenueEntry.ModifiedAt = DateTime.Now;

            // TODO:
            // Get UserId from JWT Claims
            // revenueEntry.ModifiedBy = loggedInUserId;

            await db.SaveChangesAsync();

            // Clear Cache
            cache.Remove(RevenueCacheKey);
            cache.Remove($"RevenueEntry_{revenueEntryId}");

            // Fetch Updated Record
            var updatedRevenue = await db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (updatedRevenue == null)
                throw new Exception("Revenue Entry not found after update.");

            var response = mapper.Map<RevenueEntryResponseDto>(updatedRevenue);

            return ApiResponseHelper.SuccessRes(
                response,
                "Revenue Entry marked as Invoiced successfully.");
        }

        //public async Task<ApiResponse<RevenueEntryResponseDto>> RejectAsync(int revenueEntryId, string? remarks = null)
        //{
        //    var revenueEntry = await db.RevenueEntries
        //        .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

        //    if (revenueEntry == null)
        //        throw new Exception("Revenue Entry not found.");

        //    if (revenueEntry.Status == "Approved")
        //        throw new Exception("Approved Revenue Entry cannot be rejected.");

        //    if (revenueEntry.Status == "Rejected")
        //        throw new Exception("Revenue Entry is already rejected.");

        //    if (revenueEntry.Status == "Deleted")
        //        throw new Exception("Deleted Revenue Entry cannot be rejected.");

        //    revenueEntry.Status = "Rejected";
        //    revenueEntry.ModifiedAt = DateTime.Now;

        //    // TODO:
        //    // revenueEntry.ModifiedBy = managerUserId;

        //    // Future:
        //    // revenueEntry.RejectionRemarks = remarks;

        //    await db.SaveChangesAsync();

        //    cache.Remove(RevenueCacheKey);
        //    cache.Remove($"RevenueEntry_{revenueEntryId}");

        //    var updatedRevenue = await db.RevenueEntries
        //        .AsNoTracking()
        //        .Include(x => x.Customer)
        //        .Include(x => x.Department)
        //        .Include(x => x.AccountMaster)
        //        .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

        //    var response = mapper.Map<RevenueEntryResponseDto>(updatedRevenue);

        //    return ApiResponseHelper.SuccessRes(
        //        response,
        //        "Revenue Entry rejected successfully.");
        //}




        public async Task<ApiResponse<RevenueEntryResponseDto>> MarkAsReceivedAsync(int revenueEntryId)
        {
            // Check Revenue Entry
            var revenueEntry = await db.RevenueEntries
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (revenueEntry == null)
                throw new Exception("Revenue Entry not found.");

            // Already Received
            if (revenueEntry.Status == "Received")
                throw new Exception("Revenue Entry is already marked as Received.");

            // Must be Invoiced before Received
            if (revenueEntry.Status == "Pending")
                throw new Exception("Revenue Entry must be Invoiced before it can be marked as Received.");

            // Deleted
            if (revenueEntry.Status == "Deleted")
                throw new Exception("Deleted Revenue Entry cannot be marked as Received.");

            revenueEntry.Status = "Received";
            revenueEntry.ModifiedAt = DateTime.Now;

            // TODO:
            // Get UserId from JWT Claims
            // revenueEntry.ModifiedBy = loggedInUserId;

            await db.SaveChangesAsync();

            // Clear Cache
            cache.Remove(RevenueCacheKey);
            cache.Remove($"RevenueEntry_{revenueEntryId}");

            // Fetch Updated Record
            var updatedRevenue = await db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .FirstOrDefaultAsync(x => x.RevenueEntryId == revenueEntryId);

            if (updatedRevenue == null)
                throw new Exception("Revenue Entry not found after update.");

            var response = mapper.Map<RevenueEntryResponseDto>(updatedRevenue);

            return ApiResponseHelper.SuccessRes(
                response,
                "Revenue Entry marked as Received successfully.");
        }


        public async Task<ApiResponse<List<RevenueEntryResponseDto>>> GetRevenueByStatusAsync(
             string status,
             int page,
             int pageSize)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Status is required.");

            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            string cacheKey = $"Revenue_Status_{status}_Page_{page}_Size_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<RevenueEntryResponseDto> cachedData))
            {
                int totalRecords = await db.RevenueEntries
                    .CountAsync(x => x.Status == status);

                return ApiResponseHelper.SuccessRes(
                    cachedData,
                    $"Revenue entries with status '{status}' fetched successfully.",
                    totalRecords);
            }

            var query = db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .Where(x => x.Status == status);

            int totalRecordsCount = await query.CountAsync();

            var revenueEntries = await query
                .OrderByDescending(x => x.RevenueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!revenueEntries.Any())
                throw new Exception($"No revenue entries found with status '{status}'.");

            var response = mapper.Map<List<RevenueEntryResponseDto>>(revenueEntries);

            cache.Set(
                cacheKey,
                response,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                response,
                $"Revenue entries with status '{status}' fetched successfully.",
                totalRecordsCount);
        }
        public async Task<ApiResponse<List<RevenueEntryResponseDto>>> GetRevenueByTypeAsync(
             string revenueType,
             int page,
             int pageSize)
        {
            if (string.IsNullOrWhiteSpace(revenueType))
                throw new Exception("Revenue Type is required.");

            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            string cacheKey = $"Revenue_Type_{revenueType}_Page_{page}_Size_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<RevenueEntryResponseDto> cachedData))
            {
                int totalRecords = await db.RevenueEntries
                    .CountAsync(x => x.RevenueType == revenueType);

                return ApiResponseHelper.SuccessRes(
                    cachedData,
                    $"Revenue entries with type '{revenueType}' fetched successfully.",
                    totalRecords);
            }

            var query = db.RevenueEntries
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .Where(x => x.RevenueType == revenueType);

            int totalRecordsCount = await query.CountAsync();

            var revenueEntries = await query
                .OrderByDescending(x => x.RevenueDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!revenueEntries.Any())
                throw new Exception($"No revenue entries found with type '{revenueType}'.");

            var response = mapper.Map<List<RevenueEntryResponseDto>>(revenueEntries);

            cache.Set(
                cacheKey,
                response,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                response,
                $"Revenue entries with type '{revenueType}' fetched successfully.",
                totalRecordsCount);
        }

        public async Task<ApiResponse<List<MonthlyRevenueDto>>> GetMonthlyRevenueAsync()
        {
            const string cacheKey = "MonthlyRevenue";

            // Check Cache
            if (cache.TryGetValue(cacheKey, out List<MonthlyRevenueDto>? cachedData))
            {
                return ApiResponseHelper.SuccessRes(
                    cachedData,
                    "Monthly revenue fetched successfully.",
                    cachedData.Count);
            }

            // Fetch Monthly Revenue
            var monthlyRevenue = await db.RevenueEntries
                .AsNoTracking()
                .Where(x => x.Status == "Invoiced" || x.Status == "Received")
                .GroupBy(x => new
                {
                    x.RevenueDate.Year,
                    x.RevenueDate.Month
                })
                .Select(g => new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                    TotalRevenue = g.Sum(x => x.Amount),
                    TotalTransactions = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // No Data Found
            if (!monthlyRevenue.Any())
            {
                return ApiResponseHelper.SuccessRes(
                    new List<MonthlyRevenueDto>(),
                    "No monthly revenue found.",
                    0);
            }

            // Store Cache
            cache.Set(
                cacheKey,
                monthlyRevenue,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                monthlyRevenue,
                "Monthly revenue fetched successfully.",
                monthlyRevenue.Count);
        }

        public async Task<ApiResponse<RevenueSummaryDto>> GetRevenueSummaryAsync()
        {
            const string cacheKey = "RevenueSummary";

            // Check Cache
            if (cache.TryGetValue(cacheKey, out RevenueSummaryDto? cachedSummary))
            {
                return ApiResponseHelper.SuccessRes(
                    cachedSummary,
                    "Revenue summary fetched successfully.");
            }

            // Fetch Summary
            var summary = await db.RevenueEntries
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new RevenueSummaryDto
                {
                    // Total Revenue (Exclude Deleted)
                    TotalRevenue = g
                        .Where(x => x.Status != "Deleted")
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    // Pending Revenue
                    PendingRevenue = g
                        .Where(x => x.Status == "Pending")
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    // Invoiced Revenue
                    InvoicedRevenue = g
                        .Where(x => x.Status == "Invoiced")
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    // Received Revenue
                    ReceivedRevenue = g
                        .Where(x => x.Status == "Received")
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    // Total Transactions
                    TotalTransactions = g.Count(x => x.Status != "Deleted"),

                    // Pending Transactions
                    PendingTransactions = g.Count(x => x.Status == "Pending"),

                    // Invoiced Transactions
                    InvoicedTransactions = g.Count(x => x.Status == "Invoiced"),

                    // Received Transactions
                    ReceivedTransactions = g.Count(x => x.Status == "Received")
                })
                .FirstOrDefaultAsync();

            if (summary == null)
            {
                summary = new RevenueSummaryDto();
            }

            // Cache Summary
            cache.Set(
                cacheKey,
                summary,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            return ApiResponseHelper.SuccessRes(
                summary,
                "Revenue summary fetched successfully.");
        }

    }
}