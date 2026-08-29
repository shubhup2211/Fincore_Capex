using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed.Authentication;
using Fincore.Infrastructure.Seed.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Budget
{
    /// <summary>Phase 5 – BudgetCategory ➜ Budget ➜ BudgetLine</summary>
    public static class BudgetSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedBudgetCategoriesAsync(db);
            await SeedBudgetsAsync(db);
            await SeedBudgetLinesAsync(db);
        }

        private static async Task SeedBudgetCategoriesAsync(AppDbContext db)
        {
            if (await db.BudgetCategories.AnyAsync()) return;

            var departments = await db.Departments.OrderBy(d => d.DepartmentId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;

            var catNames = new[] { "Capex", "Opex", "Salary", "Marketing", "Travel", "Training", "Software", "Hardware", "Utilities", "MaintenanceR" };
            var list = new List<BudgetCategory>();
            for (int i = 0; i < 20; i++)
            {
                var dept = departments[i % departments.Count];
                var name = catNames[i % catNames.Length];
                list.Add(new BudgetCategory
                {
                    CategoryName = $"{name}-{i + 1:D2}",
                    DepartmentId = dept.DepartmentId,
                    IsActive     = 1,
                    CreatedAt    = now,
                    ModifiedAt   = now,
                    CreatedBy    = admin,
                    ModifiedBy   = admin
                });
            }
            db.BudgetCategories.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedBudgetsAsync(AppDbContext db)
        {
            if (await db.Budgets.AnyAsync()) return;

            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 6);

            var list = new List<Fincore.Domain.Models.Budget>();
            string[] fys = { "FY2024-25", "FY2025-26" };
            string[] names = { "Annual Operating", "Capex Plan", "Marketing Spend", "IT Modernization", "Talent Acquisition", "R&D Investment", "Expansion Plan", "Facilities Upgrade", "Training Program", "Contingency Reserve" };

            for (int i = 0; i < 10; i++)
            {
                var fy = fys[i % fys.Length];
                list.Add(new Fincore.Domain.Models.Budget
                {
                    BudgetCode     = $"BUD-{fy.Substring(2, 4)}-{i + 1:D3}",
                    BudgetName     = names[i % names.Length],
                    FinancialYear  = fy,
                    StartDate      = new DateTime(2024 + (i % 2), 4, 1),
                    EndDate        = new DateTime(2025 + (i % 2), 3, 31),
                    BudgetAmount   = System.Math.Round((decimal)rng.Double(500000, 25000000), 2),
                    IsActive       = 1,
                    CreatedAt      = now,
                    ModifiedAt     = now,
                    CreatedBy      = admin,
                    ModifiedBy     = admin
                });
            }
            db.Budgets.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedBudgetLinesAsync(AppDbContext db)
        {
            if (await db.BudgetLines.AnyAsync()) return;

            var budgets    = await db.Budgets.OrderBy(b => b.BudgetId).ToListAsync();
            var vendorCategories = await db.VendorCategories.OrderBy(v => v.VendorCategoryId).ToListAsync();
            var departments = await db.Departments.OrderBy(d => d.DepartmentId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 7);

            var list = new List<BudgetLine>();

            for (int i = 0; i < 25; i++)
            {
                var budget = budgets[i % budgets.Count];
                var vendorCategory = vendorCategories[i % vendorCategories.Count];
                var department = departments[i % departments.Count];

                var allocated = Math.Round((decimal)rng.Double(25000, 500000), 2);
                var utilized = Math.Round(allocated * (decimal)rng.Double(0.1, 0.9), 2);

                list.Add(new BudgetLine
                {
                    BudgetId = budget.BudgetId,
                    VendorCategoryId = vendorCategory.VendorCategoryId,
                    DepartmentId = department.DepartmentId,

                    AllocatedAmount = allocated,
                    UtilizedAmount = utilized,
                    RemainingAmount = allocated - utilized,

                    IsActive = 1,
                    CreatedAt = now,
                    ModifiedAt = now,
                    CreatedBy = admin,
                    ModifiedBy = admin
                });
            }

            db.BudgetLines.AddRange(list);
            await db.SaveChangesAsync();
        }
    }
}
