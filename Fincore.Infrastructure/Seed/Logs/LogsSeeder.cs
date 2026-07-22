using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Logs
{
    /// <summary>Phase 8 – AuditLog, UserActivityLog, NotificationLog, ApprovalLog</summary>
    public static class LogsSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedAuditLogsAsync(db);
            await SeedUserActivityLogsAsync(db);
            await SeedNotificationLogsAsync(db);
            await SeedApprovalLogsAsync(db);
        }

        private static async Task SeedAuditLogsAsync(AppDbContext db)
        {
            if (await db.AuditLogs.AnyAsync()) return;
            var users = await db.Users.OrderBy(u => u.UserId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 27);
            var now = DateTime.UtcNow;
            string[] entities = { "User", "Role", "Company", "Vendor", "PurchaseOrder", "APInvoice", "Payment", "Budget", "CapexRequest", "GRN" };
            string[] ops = { "CREATE", "UPDATE", "DELETE", "APPROVE", "REJECT" };

            var list = new List<AuditLog>();
            for (int i = 0; i < 30; i++)
            {
                list.Add(new AuditLog
                {
                    EntityName     = entities[i % entities.Length],
                    EntityId       = rng.Int(1, 25),
                    OperationType  = ops[i % ops.Length],
                    OldData        = "{\"status\":\"Pending\"}",
                    NewData        = "{\"status\":\"Approved\"}",
                    AuditBy        = users[i % users.Count].UserId,
                    AuditAt        = now.AddDays(-rng.Int(1, 180))
                });
            }
            db.AuditLogs.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedUserActivityLogsAsync(AppDbContext db)
        {
            if (await db.UserActivityLogs.AnyAsync()) return;
            var users = await db.Users.OrderBy(u => u.UserId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 28);
            var now = DateTime.UtcNow;
            string[] activities = { "LOGIN", "LOGOUT", "VIEW_DASHBOARD", "EXPORT_REPORT", "APPROVE_PO", "SUBMIT_PR", "UPDATE_PROFILE", "RESET_PASSWORD" };
            string[] modules    = { "Auth", "Procurement", "Finance", "HR", "Budget", "Reports" };

            var list = new List<UserActivityLog>();
            for (int i = 0; i < 30; i++)
            {
                list.Add(new UserActivityLog
                {
                    UserId        = users[i % users.Count].UserId,
                    ActivityType  = activities[i % activities.Length],
                    Module        = modules[i % modules.Length],
                    ActivityDate  = now.AddDays(-rng.Int(0, 90))
                });
            }
            db.UserActivityLogs.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedNotificationLogsAsync(AppDbContext db)
        {
            if (await db.NotificationLogs.AnyAsync()) return;
            var users = await db.Users.OrderBy(u => u.UserId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 29);
            var now = DateTime.UtcNow;
            (string title, string msg)[] templates =
            {
                ("PO Approved",         "Your Purchase Order has been approved by Finance."),
                ("Expense Rejected",    "Your expense claim requires additional documentation."),
                ("Budget Threshold",    "Department budget utilization crossed 80%."),
                ("Payment Received",    "Customer payment against ARINV received."),
                ("Vendor Verified",     "Vendor KYC completed successfully."),
                ("Password Reset",      "A password reset was requested for your account."),
                ("New Message",         "You have a new message in your inbox."),
                ("Invoice Overdue",     "Vendor invoice is past its due date."),
                ("Report Generated",    "Your monthly finance report is ready."),
                ("Approval Pending",    "An item is waiting for your approval.")
            };

            var list = new List<NotificationLog>();
            for (int i = 0; i < 30; i++)
            {
                var t = templates[i % templates.Length];
                list.Add(new NotificationLog
                {
                    UserId  = users[i % users.Count].UserId,
                    Title   = t.title,
                    Message = t.msg,
                    SentAt  = now.AddDays(-rng.Int(0, 60))
                });
            }
            db.NotificationLogs.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedApprovalLogsAsync(AppDbContext db)
        {
            if (await db.ApprovalLogs.AnyAsync()) return;
            var users = await db.Users.OrderBy(u => u.UserId).ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 30);
            var now = DateTime.UtcNow;
            string[] entities = { "PurchaseRequisition", "PurchaseOrder", "CapexRequest", "OpexRequest", "ExpenseClaim", "APInvoice", "Payment" };
            string[] statuses = { "Approved", "Rejected", "Escalated", "OnHold" };

            var list = new List<ApprovalLog>();
            for (int i = 0; i < 30; i++)
            {
                list.Add(new ApprovalLog
                {
                    EntityName  = entities[i % entities.Length],
                    EntityId    = rng.Int(1, 25),
                    ApproverId  = users[i % users.Count].UserId,
                    Status      = statuses[i % statuses.Length],
                    Remarks     = i % 3 == 0 ? "Approved with standard terms." : (i % 3 == 1 ? "Rejected – incomplete documentation." : "Escalated to next level."),
                    ActionDate  = now.AddDays(-rng.Int(0, 90))
                });
            }
            db.ApprovalLogs.AddRange(list);
            await db.SaveChangesAsync();
        }
    }
}
