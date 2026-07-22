using AutoMapper;
using Fincore.Application.DTO.Reports;
using Fincore.Application.Interfaces;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        public ReportService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        // Revenue
        public async Task<List<RevenueReportDTO>> GetRevenueAsync()
        {
            var revenue = await db.RevenueEntries
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .Include(x => x.AccountMaster)
                .ToListAsync();

            return mapper.Map<List<RevenueReportDTO>>(revenue);
        }

        // Expense
        public async Task<List<ExpenseReportDTO>> GetExpenseAsync()
        {
            var expense = await db.ExpenseClaims
                .Include(x => x.OpexRequest)
                .Include(x => x.ClaimByUser)
                .ToListAsync();

            return mapper.Map<List<ExpenseReportDTO>>(expense);
        }

        // Vendpr
        public async Task<List<VendorSpendDTO>> GetVendorSpendAsync()
        {
            var payments = await db.Payments
                .Where(x => x.VendorId != null)
                .Include(x => x.Vendor).ThenInclude(x => x.Company)
                .ToListAsync();

            return mapper.Map<List<VendorSpendDTO>>(payments);
        }

        //Capex
        public async Task<List<CapexReportDTO>> GetCapexAsync()
        {
            var capex = await db.CapexRequests
                .Include(x => x.Department)
                .ToListAsync();

            return mapper.Map<List<CapexReportDTO>>(capex);
        }

        // Opex
        public async Task<List<OpexReportDTO>> GetOpexAsync()
        {
            var opex = await db.OpexRequests.ToListAsync();

            return mapper.Map<List<OpexReportDTO>>(opex);
        }

        // budget variance
        public async Task<List<BudgetVarianceReportDTO>> GetBudgetVarianceAsync()
        {
            var budget = await db.BudgetLines
                .Include(x => x.Budget)
                .Include(x => x.BudgetCategory)
                    .ThenInclude(x => x.Department)
                .ToListAsync();

            return mapper.Map<List<BudgetVarianceReportDTO>>(budget);
        }

        // Profit-Loss
        public async Task<ProfitLossReportDTO> GetProfitLossAsync()
        {
            decimal totalRevenue = await db.RevenueEntries
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal totalExpense = await db.ExpenseClaims
                .SumAsync(x => (decimal?)x.ExpenseAmount) ?? 0;

            return new ProfitLossReportDTO
            {
                TotalRevenue = totalRevenue,
                TotalExpense = totalExpense,
                NetProfitOrLoss = totalRevenue - totalExpense
            };
        }

        // Cash Flow 
        public async Task<CashFlowReportDTO> GetCashFlowAsync()
        {
            decimal totalCashInflow = await db.RevenueEntries
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal totalCashOutflow = await db.Payments
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            return new CashFlowReportDTO
            {
                TotalCashInflow = totalCashInflow,
                TotalCashOutflow = totalCashOutflow,
                NetCashFlow = totalCashInflow - totalCashOutflow
            };
        }

    }
}
