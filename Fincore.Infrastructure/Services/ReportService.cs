using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Reports;
using Fincore.Application.Interfaces;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public ReportService(AppDbContext db, IMapper mapper, IMemoryCache memoryCache)
        {
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        // Revenue
        public async Task<ApiResponse<List<RevenueReportDTO>>> GetRevenueAsync(int page, int pageSize)
        {
            string cacheKey = $"Revenue_{page}_{pageSize}";

            List<RevenueReportDTO> revenueList;

            if (memoryCache.TryGetValue(cacheKey, out revenueList))
            {
                return ApiResponseHelper.SuccessRes(
                    revenueList,
                    "Revenue Report Fetched Successfully",
                    revenueList.Count,
                    new { page, pageSize });
            }

            var revenue = await db.RevenueEntries
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!revenue.Any())
            {
                return ApiResponseHelper.Failure<List<RevenueReportDTO>>(
                    "Revenue Report Not Found",
                    "REVENUE_NOT_FOUND",
                    "No Revenue Records Found");
            }

            revenueList = mapper.Map<List<RevenueReportDTO>>(revenue);

            memoryCache.Set(cacheKey, revenueList, TimeSpan.FromMinutes(5));

            return ApiResponseHelper.SuccessRes(
                revenueList,
                "Revenue Report Fetched Successfully",
                revenueList.Count,
                new { page, pageSize });
        }


        // Expense
        public async Task<ApiResponse<List<ExpenseReportDTO>>> GetExpenseAsync(int page, int pageSize)
        {
            string cacheKey = $"Expense_{page}_{pageSize}";

            List<ExpenseReportDTO> expenseList;

            if (memoryCache.TryGetValue(cacheKey, out expenseList))
            {
                return ApiResponseHelper.SuccessRes(
                    expenseList,
                    "Expense Report Fetched Successfully",
                    expenseList.Count,
                    new { page, pageSize });
            }

            var expense = await db.ExpenseClaims
            .Include(x => x.OpexRequest).ThenInclude(x => x.BudgetLine).ThenInclude(x => x.BudgetCategory).ThenInclude(x => x.Department)
            .Include(x => x.ClaimByUser)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            if (!expense.Any())
            {
                return ApiResponseHelper.Failure<List<ExpenseReportDTO>>(
                    "Expense Report Not Found",
                    "EXPENSE_NOT_FOUND",
                    "No Expense Records Found");
            }

            expenseList = mapper.Map<List<ExpenseReportDTO>>(expense);

            memoryCache.Set(cacheKey, expenseList, TimeSpan.FromMinutes(5));

            return ApiResponseHelper.SuccessRes(
                expenseList,
                "Expense Report Fetched Successfully",
                expenseList.Count,
                new { page, pageSize });
        }

        // Vendpr
        public async Task<ApiResponse<List<VendorSpendDTO>>> GetVendorSpendAsync(int page, int pageSize)
        {
            string cacheKey = $"VendorSpend_{page}_{pageSize}";

            List<VendorSpendDTO> vendorSpendList;

            if (memoryCache.TryGetValue(cacheKey, out vendorSpendList))
            {
                return ApiResponseHelper.SuccessRes(
                    vendorSpendList,
                    "Vendor Spend Report Fetched Successfully",
                    vendorSpendList.Count,
                    new { page, pageSize });
            }

                var payments = await db.Payments
                .Where(x => x.VendorId != null)
                .Include(x => x.APInvoice)
                .Include(x => x.Vendor).ThenInclude(x => x.Company)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!payments.Any())
            {
                return ApiResponseHelper.Failure<List<VendorSpendDTO>>(
                    "Vendor Spend Report Not Found",
                    "VENDOR_SPEND_NOT_FOUND",
                    "No Vendor Spend Records Found");
            }

            vendorSpendList = mapper.Map<List<VendorSpendDTO>>(payments);

            memoryCache.Set(cacheKey, vendorSpendList, TimeSpan.FromMinutes(5));

            return ApiResponseHelper.SuccessRes(
                vendorSpendList,
                "Vendor Spend Report Fetched Successfully",
                vendorSpendList.Count,
                new { page, pageSize });
        }

        //Capex
        public async Task<ApiResponse<List<CapexReportDTO>>> GetCapexAsync(int page, int pageSize)
        {
            string cacheKey = $"Capex_{page}_{pageSize}";

            List<CapexReportDTO> capexList;

            if (memoryCache.TryGetValue(cacheKey, out capexList))
            {
                return ApiResponseHelper.SuccessRes(
                    capexList,
                    "Capex report fetched successfully.",
                    capexList.Count,
                    new { page, pageSize });
            }

            var capex = await db.CapexRequests
                .Include(x => x.Department)
                .Include(x => x.BudgetLine)
                .Include(x => x.RequestedByUser)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!capex.Any())
            {
                return ApiResponseHelper.Failure<List<CapexReportDTO>>(
                    "No Capex records found.",
                    "No Data",
                    "404");
            }

            capexList = mapper.Map<List<CapexReportDTO>>(capex);

            memoryCache.Set(cacheKey, capexList, TimeSpan.FromMinutes(5));

            return ApiResponseHelper.SuccessRes(
                capexList,
                "Capex report fetched successfully.",
                capexList.Count,
                new { page, pageSize });
        }


        // Opex
        public async Task<ApiResponse<List<OpexReportDTO>>> GetOpexAsync(int page, int pageSize)
        {
            string cacheKey = $"Opex_{page}_{pageSize}";

            List<OpexReportDTO> opexList;

            if (memoryCache.TryGetValue(cacheKey, out opexList))
            {
                return ApiResponseHelper.SuccessRes(
                    opexList,
                    "Opex report fetched successfully.",
                    opexList.Count,
                    new { page, pageSize });
            }

            var opex = await db.OpexRequests
                .Include(x => x.BudgetLine)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ExpenseClaims)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!opex.Any())
            {
                return ApiResponseHelper.Failure<List<OpexReportDTO>>(
                     "No Opex records found.",
                     "No Data",
                     "404");
            }

            opexList = mapper.Map<List<OpexReportDTO>>(opex);

            memoryCache.Set(cacheKey, opexList, TimeSpan.FromMinutes(5));

            return ApiResponseHelper.SuccessRes(
                opexList,
                "Opex report fetched successfully.",
                opexList.Count,
                new { page, pageSize });
        }

        // budget variance
        public async Task<ApiResponse<List<BudgetVarianceReportDTO>>> GetBudgetVarianceAsync(int page, int pageSize)
        {
            string cacheKey = $"BudgetVariance_{page}_{pageSize}";

            List<BudgetVarianceReportDTO> budgetVarianceList;

            if (!memoryCache.TryGetValue(cacheKey, out budgetVarianceList))
            {
                var budget = await db.BudgetLines
                    .Include(x => x.Budget)
                    .Include(x => x.BudgetCategory).ThenInclude(x => x.Department)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!budget.Any())
                {
                    return ApiResponseHelper.Failure<List<BudgetVarianceReportDTO>>(
                        "Budget Variance Report Not Found",
                        "BUDGET_VARIANCE_NOT_FOUND",
                        "No Budget Variance Records Found");
                }

                budgetVarianceList = mapper.Map<List<BudgetVarianceReportDTO>>(budget);

                memoryCache.Set(cacheKey, budgetVarianceList, TimeSpan.FromMinutes(5));
            }

            return ApiResponseHelper.SuccessRes(
                budgetVarianceList,
                "Budget Variance Report Fetched Successfully",
                budgetVarianceList.Count,
                new { page, pageSize });
        }

        // Profit-Loss
        public async Task<ApiResponse<ProfitLossReportDTO>> GetProfitLossAsync(
    int? companyId,
    int? departmentId,
    DateTime? fromDate,
    DateTime? toDate)
        {
            string cacheKey = $"ProfitLoss_{companyId}_{departmentId}_{fromDate}_{toDate}";

            if (memoryCache.TryGetValue(cacheKey, out ApiResponse<ProfitLossReportDTO> cachedResponse))
            {
                return cachedResponse;
            }

            // Revenue Query
            var revenueQuery = db.RevenueEntries
                .Include(r => r.Department)
                    .ThenInclude(d => d.Company)
                .AsQueryable();

            // Expense Query
            var expenseQuery = db.ExpenseClaims
                .Include(e => e.OpexRequest)
                    .ThenInclude(o => o.BudgetLine)
                        .ThenInclude(b => b.BudgetCategory)
                            .ThenInclude(c => c.Department)
                                .ThenInclude(d => d.Company)
                .AsQueryable();

            // Company Filter
            if (companyId.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.Department.CompanyId == companyId.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.OpexRequest.BudgetLine.BudgetCategory.Department.CompanyId == companyId.Value);
            }

            // Department Filter
            if (departmentId.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.DepartmentId == departmentId.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.OpexRequest.BudgetLine.BudgetCategory.Department.DepartmentId == departmentId.Value);
            }

            // Date Filter
            if (fromDate.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.RevenueDate >= fromDate.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.ExpenseDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.RevenueDate <= toDate.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.ExpenseDate <= toDate.Value);
            }

            decimal totalRevenue = await revenueQuery.SumAsync(r => (decimal?)r.Amount) ?? 0;

            decimal totalExpense = await expenseQuery.SumAsync(e => (decimal?)e.ExpenseAmount) ?? 0;

            decimal profit = 0;
            decimal loss = 0;

            if (totalRevenue > totalExpense)
            {
                profit = totalRevenue - totalExpense;
            }
            else if (totalExpense > totalRevenue)
            {
                loss = totalExpense - totalRevenue;
            }

            string companyName = "All Companies";

            if (companyId.HasValue)
            {
                companyName = await db.Companies
                    .Where(c => c.CompanyId == companyId.Value)
                    .Select(c => c.CompanyName)
                    .FirstOrDefaultAsync() ?? "N/A";
            }

            string departmentName = "All Departments";

            if (departmentId.HasValue)
            {
                departmentName = await db.Departments
                    .Where(d => d.DepartmentId == departmentId.Value)
                    .Select(d => d.DepartmentName)
                    .FirstOrDefaultAsync() ?? "N/A";
            }

            var result = new ProfitLossReportDTO
            {
                CompanyName = companyName,
                DepartmentName = departmentName,
                TotalRevenue = totalRevenue,
                TotalExpense = totalExpense,
                Profit = profit,
                Loss = loss
            };

            var response = ApiResponseHelper.SuccessRes(
                result,
                "Profit & Loss report fetched successfully.");

            memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }


        // Cash Flow
        public async Task<ApiResponse<CashFlowReportDTO>> GetCashFlowAsync(
    int? companyId,
    int? departmentId,
    DateTime? fromDate,
    DateTime? toDate)
        {
            string cacheKey = $"CashFlow_{companyId}_{departmentId}_{fromDate}_{toDate}";

            if (memoryCache.TryGetValue(cacheKey, out ApiResponse<CashFlowReportDTO> cachedResponse))
            {
                return cachedResponse;
            }

            // Revenue Query (Cash Inflow)
            var revenueQuery = db.RevenueEntries
                .Include(r => r.Department)
                    .ThenInclude(d => d.Company)
                .AsQueryable();

            // Expense Query (Cash Outflow)
            var expenseQuery = db.ExpenseClaims
                .Include(e => e.OpexRequest)
                    .ThenInclude(o => o.BudgetLine)
                        .ThenInclude(b => b.BudgetCategory)
                            .ThenInclude(c => c.Department)
                                .ThenInclude(d => d.Company)
                .AsQueryable();

            // Company Filter
            if (companyId.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.Department.CompanyId == companyId.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.OpexRequest.BudgetLine.BudgetCategory.Department.CompanyId == companyId.Value);
            }

            // Department Filter
            if (departmentId.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.DepartmentId == departmentId.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.OpexRequest.BudgetLine.BudgetCategory.Department.DepartmentId == departmentId.Value);
            }

            // Date Filter
            if (fromDate.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.RevenueDate >= fromDate.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.ExpenseDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                revenueQuery = revenueQuery.Where(r =>
                    r.RevenueDate <= toDate.Value);

                expenseQuery = expenseQuery.Where(e =>
                    e.ExpenseDate <= toDate.Value);
            }

            decimal cashInflow = await revenueQuery.SumAsync(r => (decimal?)r.Amount) ?? 0;

            decimal cashOutflow = await expenseQuery.SumAsync(e => (decimal?)e.ExpenseAmount) ?? 0;

            decimal netCashFlow = cashInflow - cashOutflow;

            string companyName = "All Companies";

            if (companyId.HasValue)
            {
                companyName = await db.Companies
                    .Where(c => c.CompanyId == companyId.Value)
                    .Select(c => c.CompanyName)
                    .FirstOrDefaultAsync() ?? "N/A";
            }

            string departmentName = "All Departments";

            if (departmentId.HasValue)
            {
                departmentName = await db.Departments
                    .Where(d => d.DepartmentId == departmentId.Value)
                    .Select(d => d.DepartmentName)
                    .FirstOrDefaultAsync() ?? "N/A";
            }

            string status;

            if (netCashFlow > 0)
                status = "Positive Cash Flow";
            else if (netCashFlow < 0)
                status = "Negative Cash Flow";
            else
                status = "Break Even";

            var result = new CashFlowReportDTO
            {
                CompanyName = companyName,
                DepartmentName = departmentName,
                CashInflow = cashInflow,
                CashOutflow = cashOutflow,
                NetCashFlow = netCashFlow,
                CashFlowStatus = status
            };

            var response = ApiResponseHelper.SuccessRes(
                result,
                "Cash Flow report fetched successfully.");

            memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }

        public async Task<ApiResponse<List<BalanceSheetReportDTO>>> GetBalanceSheetAsync(int page, int pageSize)
        {
            string cacheKey = $"BalanceSheet_{page}_{pageSize}";

            if (memoryCache.TryGetValue(cacheKey, out ApiResponse<List<BalanceSheetReportDTO>> cachedResponse))
            {
                return cachedResponse;
            }

            var journalEntries = await db.JournalEntries
                .Include(x => x.AccountMaster)
                .OrderBy(x => x.EntryDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!journalEntries.Any())
            {
                return ApiResponseHelper.Failure<List<BalanceSheetReportDTO>>(
                    "No balance sheet records found.",
                    "BALANCE_SHEET_NOT_FOUND",
                    "No journal entries available.");
            }

            var result = mapper.Map<List<BalanceSheetReportDTO>>(journalEntries);

            var response = ApiResponseHelper.SuccessRes(
                result,
                "Balance Sheet report fetched successfully.",
                result.Count);

            memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

            return response;
        }
    }
}
