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

namespace Fincore.Infrastructure.Seed.Finance
{
    /// <summary>Phase 7 – Asset ➜ Opex ➜ Expense ➜ WorkOrder ➜ AccountMaster ➜ Revenue ➜ APInvoice ➜ ARInvoice ➜ Payment ➜ JournalEntry</summary>
    public static class FinanceSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedAssetsAsync(db);
            await SeedOpexRequestsAsync(db);
            await SeedExpenseClaimsAsync(db);
            await SeedWorkOrdersAsync(db);
            await SeedAccountMastersAsync(db);
            await SeedRevenueEntriesAsync(db);
            await SeedAPInvoicesAsync(db);
            await SeedARInvoicesAsync(db);
            await SeedPaymentsAsync(db);
            await SeedJournalEntriesAsync(db);
        }

        private static async Task SeedAssetsAsync(AppDbContext db)
        {
            if (await db.Assets.AnyAsync()) return;
            var capex = await db.CapexRequests.Where(c => c.ApprovalStatus == "Approved").ToListAsync();
            var pos = await db.PurchaseOrders.Include(p => p.Quotation).OrderBy(p => p.POId).ToListAsync();
            var grns = await db.GRNs.OrderBy(g => g.GRNId).ToListAsync();
            var depts = await db.Departments.OrderBy(d => d.DepartmentId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 18);
            var now = DateTime.UtcNow;
            string[] statuses = { "InUse", "InStorage", "Maintenance", "Retired" };
            string[] names = { "Dell Latitude 7440","HP EliteBook","Lenovo ThinkStation","APC UPS 3KVA","Cisco Switch","Poly Video Bar","Epson Printer","Meeting Table","Ergo Chair","Server Rack","Storage NAS","UPS Battery","AC Unit","Whiteboard","Projector","Coffee Machine","Fire Extinguisher","Barcode Scanner","Access Point","IP Phone" };

            var list = new List<Asset>();
            for (int i = 0; i < System.Math.Min(20, pos.Count); i++)
            {
                var po = pos[i];
                var grn = grns.FirstOrDefault(g => g.POId == po.POId);
                list.Add(new Asset
                {
                    AssetCode        = $"AST-{DateTime.UtcNow.Year}-{i + 1:D4}",
                    AssetName        = names[i % names.Length],
                    CapexRequestId   = capex.Count > 0 ? capex[i % capex.Count].CapexRequestId : (int?)null,
                    PurchaseOrderId  = po.POId,
                    GRNId            = grn?.GRNId,
                    VendorId         = po.Quotation.VendorId,
                    DepartmentId     = depts[i % depts.Count].DepartmentId,
                    PurchaseDate     = now.AddDays(-rng.Int(30, 400)),
                    PurchaseCost     = System.Math.Round((decimal)rng.Double(15000, 500000), 2),
                    Status           = statuses[i % statuses.Length],
                    CreatedAt        = now.AddDays(-rng.Int(1, 200)),
                    ModifiedAt       = now
                });
            }
            db.Assets.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedOpexRequestsAsync(AppDbContext db)
        {
            if (await db.OpexRequests.AnyAsync()) return;
            var lines = await db.BudgetLines.OrderBy(b => b.BudgetLineId).ToListAsync();
            var users = await db.Users.Where(u => u.UserCategory == "Employee").ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 19);
            var now = DateTime.UtcNow;
            string[] statuses = { "Approved", "Approved", "Pending", "Rejected" };

            var list = new List<OpexRequest>();
            for (int i = 0; i < 20; i++)
            {
                var s = statuses[i % statuses.Length];
                list.Add(new OpexRequest
                {
                    BudgetLineId    = lines[i % lines.Count].BudgetLineId,
                    Title           = $"Opex Item {i + 1:D2}",
                    Amount          = System.Math.Round((decimal)rng.Double(5000, 150000), 2),
                    RequestedBy     = users[i % users.Count].UserId,
                    ApprovalStatus  = s,
                    ApprovedBy      = s == "Approved" ? admin : (int?)null,
                    ApprovedAt      = s == "Approved" ? now.AddDays(-rng.Int(1, 30)) : (DateTime?)null,
                    CreatedAt       = now.AddDays(-rng.Int(10, 100)),
                    ModifiedAt      = now
                });
            }
            db.OpexRequests.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedExpenseClaimsAsync(AppDbContext db)
        {
            if (await db.ExpenseClaims.AnyAsync()) return;
            var ops = await db.OpexRequests.OrderBy(o => o.OpexRequestId).ToListAsync();
            var users = await db.Users.Where(u => u.UserCategory == "Employee").ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 20);
            var now = DateTime.UtcNow;
            string[] types = { "Travel", "Meals", "Office Supplies", "Fuel", "Accommodation", "Training", "Client Entertainment" };
            string[] statuses = { "Approved", "Approved", "Pending", "Rejected" };

            var list = new List<ExpenseClaim>();
            for (int i = 0; i < 20; i++)
            {
                var s = statuses[i % statuses.Length];
                list.Add(new ExpenseClaim
                {
                    ClaimNumber     = $"EXP-{DateTime.UtcNow.Year}-{i + 1:D4}",
                    Description     = $"Reimbursement claim #{i + 1} – business expense",
                    OpexRequestId   = ops[i % ops.Count].OpexRequestId,
                    ExpenseDate     = now.AddDays(-rng.Int(1, 60)),
                    ExpenseType     = types[i % types.Length],
                    ExpenseAmount   = System.Math.Round((decimal)rng.Double(500, 25000), 2),
                    ClaimBy         = users[i % users.Count].UserId,
                    ApprovalStatus  = s,
                    ApprovedBy      = s == "Approved" ? admin : (int?)null,
                    CreatedAt       = now.AddDays(-rng.Int(1, 60)),
                    ModifiedAt      = now
                });
            }
            db.ExpenseClaims.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedWorkOrdersAsync(AppDbContext db)
        {
            if (await db.WorkOrders.AnyAsync()) return;
            var vendors = await db.Vendors.OrderBy(v => v.VendorId).ToListAsync();
            var ops     = await db.OpexRequests.Where(o => o.ApprovalStatus == "Approved").OrderBy(o => o.OpexRequestId).ToListAsync();
            if (ops.Count == 0) ops = await db.OpexRequests.OrderBy(o => o.OpexRequestId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 21);
            var now = DateTime.UtcNow;
            string[] statuses = { "Open", "InProgress", "Completed", "OnHold" };

            var list = new List<WorkOrder>();
            for (int i = 0; i < 20; i++)
            {
                var s = statuses[i % statuses.Length];
                var start = now.AddDays(-rng.Int(5, 60));
                list.Add(new WorkOrder
                {
                    WONumber       = $"WO-{DateTime.UtcNow.Year}-{i + 1:D4}",
                    Title          = $"Work Order {i + 1:D2}",
                    VendorId       = vendors[i % vendors.Count].VendorId,
                    OpexRequestId  = ops[i % ops.Count].OpexRequestId,
                    NetAmount      = System.Math.Round((decimal)rng.Double(10000, 350000), 2),
                    Status         = s,
                    StartDate      = start,
                    CompletedDate  = s == "Completed" ? start.AddDays(rng.Int(3, 30)) : (DateTime?)null,
                    CreatedBy      = admin,
                    CreatedDate    = start
                });
            }
            db.WorkOrders.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedAccountMastersAsync(AppDbContext db)
        {
            if (await db.AccountMasters.AnyAsync()) return;
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;

            var accounts = new (string code, string name, string type)[]
            {
                ("1000","Cash on Hand","Asset"),
                ("1010","Bank – HDFC Current","Asset"),
                ("1020","Bank – ICICI Current","Asset"),
                ("1200","Accounts Receivable","Asset"),
                ("1500","Fixed Assets – IT","Asset"),
                ("1510","Fixed Assets – Furniture","Asset"),
                ("2000","Accounts Payable","Liability"),
                ("2100","GST Payable","Liability"),
                ("2200","TDS Payable","Liability"),
                ("3000","Share Capital","Equity"),
                ("3100","Retained Earnings","Equity"),
                ("4000","Sales Revenue","Revenue"),
                ("4100","Service Revenue","Revenue"),
                ("5000","Salaries Expense","Expense"),
                ("5100","Rent Expense","Expense"),
                ("5200","Utilities Expense","Expense"),
                ("5300","Travel Expense","Expense"),
                ("5400","Software Subscriptions","Expense")
            };
            db.AccountMasters.AddRange(accounts.Select(a => new AccountMaster
            {
                AccountCode = a.code,
                AccountName = a.name,
                AccountType = a.type,
                IsActive    = 1,
                CreatedAt   = now,
                ModifiedAt  = now,
                CreatedBy   = admin,
                ModifiedBy  = admin
            }));
            await db.SaveChangesAsync();
        }

        private static async Task SeedRevenueEntriesAsync(AppDbContext db)
        {
            if (await db.RevenueEntries.AnyAsync()) return;
            var customers = await db.Customers.OrderBy(c => c.CustomerId).ToListAsync();
            var depts     = await db.Departments.OrderBy(d => d.DepartmentId).ToListAsync();
            var accts     = await db.AccountMasters.Where(a => a.AccountType == "Revenue").ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 22);
            var now = DateTime.UtcNow;
            string[] types = { "Product", "Service", "Subscription", "License", "Consulting" };
            string[] statuses = { "Invoiced", "Received", "Pending" };

            var list = new List<RevenueEntry>();
            for (int i = 0; i < 25; i++)
            {
                list.Add(new RevenueEntry
                {
                    InvoiceNumber  = $"REV-{DateTime.UtcNow.Year}-{i + 1:D5}",
                    CustomerId     = customers[i % customers.Count].CustomerId,
                    DepartmentId   = depts[i % depts.Count].DepartmentId,
                    RevenueType    = types[i % types.Length],
                    Amount         = System.Math.Round((decimal)rng.Double(20000, 900000), 2),
                    RevenueDate    = now.AddDays(-rng.Int(1, 180)),
                    AccountId      = accts[i % accts.Count].AccountId,
                    Status         = statuses[i % statuses.Length],
                    CreatedAt      = now.AddDays(-rng.Int(1, 180)),
                    ModifiedAt     = now,
                    CreatedBy      = admin,
                    ModifiedBy     = admin
                });
            }
            db.RevenueEntries.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedAPInvoicesAsync(AppDbContext db)
        {
            if (await db.APInvoices.AnyAsync()) return;
            var grns = await db.GRNs.Include(g => g.PurchaseOrder).OrderBy(g => g.GRNId).ToListAsync();
            var wos  = await db.WorkOrders.OrderBy(w => w.WorkOrderId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 23);
            var now = DateTime.UtcNow;
            string[] appStatus = { "Approved", "Pending", "Rejected" };
            string[] payStatus = { "Paid", "PartiallyPaid", "Unpaid" };

            var list = new List<APInvoice>();
            for (int i = 0; i < System.Math.Min(20, grns.Count); i++)
            {
                var g = grns[i];
                var inv = now.AddDays(-rng.Int(5, 60));
                list.Add(new APInvoice
                {
                    InvoiceNumber   = $"APINV-{DateTime.UtcNow.Year}-{i + 1:D5}",
                    VendorId        = g.VendorId,
                    PurchaseOrderId = g.POId,
                    GRNId           = g.GRNId,
                    WorkOrderId     = wos.Count > 0 ? wos[i % wos.Count].WorkOrderId : (int?)null,
                    InvoiceDate     = inv,
                    DueDate         = inv.AddDays(30),
                    Amount          = System.Math.Round(g.PurchaseOrder.Amount, 2),
                    InvoiceFile     = $"/uploads/apinvoices/ap_{i + 1:D5}.pdf",
                    ApprovedBy      = admin,
                    ApprovalStatus  = appStatus[i % appStatus.Length],
                    PaymentStatus   = payStatus[i % payStatus.Length],
                    CreatedAt       = inv,
                    ModifiedAt      = now
                });
            }
            db.APInvoices.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedARInvoicesAsync(AppDbContext db)
        {
            if (await db.ARInvoices.AnyAsync()) return;
            var revs = await db.RevenueEntries.OrderBy(r => r.RevenueEntryId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 24);
            var now = DateTime.UtcNow;
            string[] payStatus = { "Paid", "PartiallyPaid", "Unpaid", "Overdue" };

            var list = new List<ARInvoice>();
            for (int i = 0; i < System.Math.Min(25, revs.Count); i++)
            {
                var r = revs[i];
                var inv = r.RevenueDate;
                var received = System.Math.Round(r.Amount * (decimal)rng.Double(0, 1), 2);
                list.Add(new ARInvoice
                {
                    InvoiceNumber     = $"ARINV-{DateTime.UtcNow.Year}-{i + 1:D5}",
                    CustomerId        = r.CustomerId,
                    RevenueEntryId    = r.RevenueEntryId,
                    InvoiceDate       = inv,
                    DueDate           = inv.AddDays(45),
                    Amount            = r.Amount,
                    AmountReceived    = received,
                    AmountOutstanding = System.Math.Round(r.Amount - received, 2),
                    PaymentStatus     = payStatus[i % payStatus.Length],
                    CreatedAt         = inv,
                    ModifiedAt        = now
                });
            }
            db.ARInvoices.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedPaymentsAsync(AppDbContext db)
        {
            if (await db.Payments.AnyAsync()) return;
            var apInvs = await db.APInvoices.OrderBy(a => a.APInvoiceId).ToListAsync();
            var arInvs = await db.ARInvoices.OrderBy(a => a.ARInvoiceId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 25);
            var now = DateTime.UtcNow;
            string[] methods = { "Bank Transfer", "Cheque", "UPI", "Credit Card", "Cash" };
            string[] appStatus = { "Approved", "Pending" };

            var list = new List<Payment>();
            int n = 0;
            // AP payments (outgoing)
            for (int i = 0; i < System.Math.Min(15, apInvs.Count); i++)
            {
                n++;
                var inv = apInvs[i];
                list.Add(new Payment
                {
                    PaymentNumber   = $"PAY-{DateTime.UtcNow.Year}-{n:D5}",
                    PaymentType     = "AP",
                    APInvoiceId     = inv.APInvoiceId,
                    VendorId        = inv.VendorId,
                    Amount          = inv.Amount,
                    PaymentDate     = inv.DueDate.AddDays(-rng.Int(0, 10)),
                    PaymentMethod   = methods[i % methods.Length],
                    ApprovedBy      = admin,
                    ReconciledFlag  = rng.Bool(),
                    ApprovalStatus  = appStatus[i % appStatus.Length],
                    CreatedAt       = inv.InvoiceDate,
                    ModifiedAt      = now
                });
            }
            // AR payments (incoming)
            for (int i = 0; i < System.Math.Min(15, arInvs.Count); i++)
            {
                n++;
                var inv = arInvs[i];
                list.Add(new Payment
                {
                    PaymentNumber   = $"PAY-{DateTime.UtcNow.Year}-{n:D5}",
                    PaymentType     = "AR",
                    ARInvoiceId     = inv.ARInvoiceId,
                    CustomerId      = inv.CustomerId,
                    Amount          = inv.AmountReceived ?? inv.Amount,
                    PaymentDate     = inv.InvoiceDate.AddDays(rng.Int(1, 40)),
                    PaymentMethod   = methods[i % methods.Length],
                    ApprovedBy      = admin,
                    ReconciledFlag  = rng.Bool(),
                    ApprovalStatus  = appStatus[i % appStatus.Length],
                    CreatedAt       = inv.InvoiceDate,
                    ModifiedAt      = now
                });
            }
            db.Payments.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedJournalEntriesAsync(AppDbContext db)
        {
            if (await db.JournalEntries.AnyAsync()) return;
            var accts = await db.AccountMasters.OrderBy(a => a.AccountId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 26);
            var now = DateTime.UtcNow;

            var list = new List<JournalEntry>();
            for (int i = 0; i < 30; i++)
            {
                var a = accts[i % accts.Count];
                var isDebit = i % 2 == 0;
                var amt = System.Math.Round((decimal)rng.Double(1000, 500000), 2);
                list.Add(new JournalEntry
                {
                    JournalNumber = $"JV-{DateTime.UtcNow.Year}-{i + 1:D5}",
                    EntryDate     = now.AddDays(-rng.Int(1, 180)),
                    AccountId     = a.AccountId,
                    DebitAmount   = isDebit  ? amt : 0,
                    CreditAmount  = !isDebit ? amt : 0,
                    Description   = $"Automated journal – posting #{i + 1} for {a.AccountName}",
                    CreatedBy     = admin,
                    CreatedAt     = now.AddDays(-rng.Int(1, 180)),
                    ModifiedAt    = now
                });
            }
            db.JournalEntries.AddRange(list);
            await db.SaveChangesAsync();
        }
    }
}
