using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Authentication
{
    /// <summary>
    /// Phase 2 – Roles seeder.
    /// Handles the Role &lt;-&gt; User circular dependency by
    /// bootstrapping the first Role &amp; SuperAdmin User via raw SQL
    /// with FK constraints temporarily disabled.
    /// After bootstrap, the remaining roles are inserted normally.
    /// </summary>
    public static class RoleSeeder
    {
        public const int BootstrapRoleId = 1;   // SuperAdmin
        public const int BootstrapUserId = 1;   // SuperAdmin user

        public static async Task BootstrapAdminAsync(AppDbContext db)
        {
            if (await db.Roles.AnyAsync() || await db.Users.AnyAsync()) return;

            // Insert the SuperAdmin Role + SuperAdmin User atomically with
            // FK constraints disabled so the circular reference resolves.
            var now = DateTime.UtcNow;
            var passwordHash = "Admin@123"; // plain – user will hash later

            var sql = $@"
                ALTER TABLE [Roles] NOCHECK CONSTRAINT ALL;
                ALTER TABLE [Users] NOCHECK CONSTRAINT ALL;

                SET IDENTITY_INSERT [Roles] ON;
                INSERT INTO [Roles] ([RoleId], [RoleName], [Description], [UserId], [IsActive], [CreatedAt], [ModifiedAt], [CreatedBy], [ModifiedBy])
                VALUES ({BootstrapRoleId}, N'SuperAdmin', N'System super administrator with unrestricted access', NULL, 1, '{now:yyyy-MM-dd HH:mm:ss}', '{now:yyyy-MM-dd HH:mm:ss}', {BootstrapUserId}, {BootstrapUserId});
                SET IDENTITY_INSERT [Roles] OFF;

                SET IDENTITY_INSERT [Users] ON;
                INSERT INTO [Users] ([UserId], [RoleId], [FullName], [Email], [PasswordHash], [User Category], [Phone], [LastLogin], [RefreshToken], [IsActive], [CreatedAt], [ModifiedAt], [CreatedBy], [ModifiedBy])
                VALUES ({BootstrapUserId}, {BootstrapRoleId}, N'Super Admin', N'admin@fincore.com', N'{passwordHash}', N'Employee', N'9000000001', '{now:yyyy-MM-dd HH:mm:ss}', N'', 1, '{now:yyyy-MM-dd HH:mm:ss}', '{now:yyyy-MM-dd HH:mm:ss}', {BootstrapUserId}, {BootstrapUserId});
                SET IDENTITY_INSERT [Users] OFF;

                ALTER TABLE [Roles] WITH CHECK CHECK CONSTRAINT ALL;
                ALTER TABLE [Users] WITH CHECK CHECK CONSTRAINT ALL;";

            await db.Database.ExecuteSqlRawAsync(sql);
        }

        public static async Task SeedRolesAsync(AppDbContext db)
        {
            // SuperAdmin already bootstrapped; only add if we still have <2 roles.
            if (await db.Roles.CountAsync() > 1) return;

            var now = DateTime.UtcNow;
            var roles = new List<Role>
            {
                new() { RoleName = "Admin",            Description = "Administrator with elevated privileges",       IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Finance Manager",  Description = "Finance module owner",                          IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Procurement Head", Description = "Procurement module owner",                      IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "HR Manager",       Description = "HR module owner",                               IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Department Head",  Description = "Head of a business department",                 IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Employee",         Description = "Regular employee",                              IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Vendor",           Description = "External vendor / supplier user",               IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Customer",         Description = "External customer user",                        IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Accountant",       Description = "General ledger / bookkeeping",                  IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId },
                new() { RoleName = "Auditor",          Description = "Read-only auditor with audit trail visibility", IsActive = 1, CreatedAt = now, ModifiedAt = now, CreatedBy = BootstrapUserId, ModifiedBy = BootstrapUserId }
            };
            db.Roles.AddRange(roles);
            await db.SaveChangesAsync();
        }
    }
}
