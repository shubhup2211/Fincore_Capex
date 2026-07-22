using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Authentication
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (await db.Permissions.AnyAsync()) return;

            var roles = await db.Roles.ToListAsync();
            var masterTypes = await db.MasterTypes.ToListAsync();
            var superAdmin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;

            int R(string n) => roles.First(r => r.RoleName == n).RoleId;
            int? MT(string n) => masterTypes.FirstOrDefault(m => m.MasterTypeName == n)?.MasterTypeId;

            var perms = new List<Permission>
            {
                new() { PermissionName = "Manage Users",            RoleId = R("SuperAdmin"),        MasterTypeId = MT("Employee") },
                new() { PermissionName = "Manage Roles",            RoleId = R("SuperAdmin"),        MasterTypeId = null },
                new() { PermissionName = "Manage Companies",        RoleId = R("Admin"),             MasterTypeId = MT("Company") },
                new() { PermissionName = "Manage Departments",      RoleId = R("Admin"),             MasterTypeId = MT("Company") },
                new() { PermissionName = "View Audit Logs",         RoleId = R("Auditor"),           MasterTypeId = null },
                new() { PermissionName = "Approve Budgets",         RoleId = R("Finance Manager"),   MasterTypeId = null },
                new() { PermissionName = "Approve Capex Requests",  RoleId = R("Finance Manager"),   MasterTypeId = null },
                new() { PermissionName = "Approve Opex Requests",   RoleId = R("Finance Manager"),   MasterTypeId = null },
                new() { PermissionName = "Approve Payments",        RoleId = R("Finance Manager"),   MasterTypeId = null },
                new() { PermissionName = "Post Journal Entries",    RoleId = R("Accountant"),        MasterTypeId = null },
                new() { PermissionName = "Manage Accounts",         RoleId = R("Accountant"),        MasterTypeId = null },
                new() { PermissionName = "Manage Vendors",          RoleId = R("Procurement Head"),  MasterTypeId = MT("Vendor") },
                new() { PermissionName = "Create Purchase Reqs",    RoleId = R("Procurement Head"),  MasterTypeId = null },
                new() { PermissionName = "Approve Purchase Orders", RoleId = R("Procurement Head"),  MasterTypeId = null },
                new() { PermissionName = "Manage RFQs",             RoleId = R("Procurement Head"),  MasterTypeId = null },
                new() { PermissionName = "Manage Employees",        RoleId = R("HR Manager"),        MasterTypeId = MT("Employee") },
                new() { PermissionName = "Approve Expense Claims",  RoleId = R("HR Manager"),        MasterTypeId = null },
                new() { PermissionName = "Manage Own Department",   RoleId = R("Department Head"),   MasterTypeId = null },
                new() { PermissionName = "Raise Purchase Req",      RoleId = R("Department Head"),   MasterTypeId = null },
                new() { PermissionName = "Submit Expense Claim",    RoleId = R("Employee"),          MasterTypeId = MT("Employee") },
                new() { PermissionName = "View Own Profile",        RoleId = R("Employee"),          MasterTypeId = MT("Employee") },
                new() { PermissionName = "Submit Quotation",        RoleId = R("Vendor"),            MasterTypeId = MT("Vendor") },
                new() { PermissionName = "View Own POs",            RoleId = R("Vendor"),            MasterTypeId = MT("Vendor") },
                new() { PermissionName = "View Own Invoices",       RoleId = R("Customer"),          MasterTypeId = MT("Customer") },
                new() { PermissionName = "Download Statements",     RoleId = R("Customer"),          MasterTypeId = MT("Customer") }
            };

            foreach (var p in perms)
            {
                p.IsActive   = 1;
                p.CreatedAt  = now;
                p.ModifiedAt = now;
                p.CreatedBy  = superAdmin;
                p.ModifiedBy = superAdmin;
            }
            db.Permissions.AddRange(perms);
            await db.SaveChangesAsync();
        }
    }
}
