using System;
using System.Threading.Tasks;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed.Authentication;
using Fincore.Infrastructure.Seed.Budget;
using Fincore.Infrastructure.Seed.Business;
using Fincore.Infrastructure.Seed.Finance;
using Fincore.Infrastructure.Seed.Logs;
using Fincore.Infrastructure.Seed.Master;
using Fincore.Infrastructure.Seed.Organization;
using Fincore.Infrastructure.Seed.Procurement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fincore.Infrastructure.Seed
{
    /// <summary>
    /// Central orchestrator that runs every phase in the correct FK-safe order.
    /// Wire it up in Program.cs (see README.md).
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("DatabaseSeeder");

            try
            {
                // Make sure DB schema is present. Comment out if you exclusively use Migrations.
                await db.Database.EnsureCreatedAsync();

                logger?.LogInformation("[Seed] Phase 1 – Master (Currency ➜ Country ➜ State ➜ City ➜ MasterType)");
                await MasterSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 2 – Authentication (bootstrap Admin ➜ Roles ➜ Users ➜ Permissions)");
                await RoleSeeder.BootstrapAdminAsync(db);
                await RoleSeeder.SeedRolesAsync(db);
                await UserSeeder.SeedAsync(db);
                await PermissionSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 3 – Organization (Company ➜ Department ➜ Employee)");
                await OrganizationSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 4 – Business (Customer, VendorCategory, Vendor, DocumentType, Document)");
                await BusinessSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 5 – Budget (BudgetCategory ➜ Budget ➜ BudgetLine)");
                await BudgetSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 6 – Procurement (ApprovalFlow ➜ Capex ➜ PR ➜ RFQ ➜ Quotation ➜ Selection ➜ PO ➜ GRN)");
                await ProcurementSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 7 – Finance (Asset ➜ Opex ➜ Expense ➜ WO ➜ Accounts ➜ Revenue ➜ APInvoice ➜ ARInvoice ➜ Payment ➜ JV)");
                await FinanceSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] Phase 8 – Logs (Audit, UserActivity, Notification, Approval)");
                await LogsSeeder.SeedAsync(db);

                logger?.LogInformation("[Seed] ✔ Database seeded successfully.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "[Seed] Database seeding failed.");
                throw;
            }
        }
    }
}
