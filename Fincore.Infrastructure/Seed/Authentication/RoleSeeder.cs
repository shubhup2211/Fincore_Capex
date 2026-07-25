using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Authentication
{
    public static class RoleSeeder
    {
        public const int BootstrapRoleId = 1;   // SuperAdmin
        public const int BootstrapUserId = 1;   // SuperAdmin user

        public static async Task BootstrapAdminAsync(AppDbContext db)
        {
            // FIX: check Role 1 and User 1 INDEPENDENTLY, not "any row anywhere".
            // This makes the seeder self-healing: if a previous run left Roles
            // seeded but Users missing (exactly your current state), this will
            // detect the gap and insert only what's missing.
            bool roleExists = await db.Roles.AnyAsync(r => r.RoleId == BootstrapRoleId);
            bool userExists = await db.Users.AnyAsync(u => u.UserId == BootstrapUserId);

            if (roleExists && userExists) return; // fully bootstrapped, nothing to do

            var now = DateTime.UtcNow;
            var passwordHash = "Admin@123"; // plain – user will hash later

            // Build only the INSERT statements that are actually needed.
            var sqlParts = new List<string>
            {
                "BEGIN TRAN;",
                "ALTER TABLE [Roles] NOCHECK CONSTRAINT ALL;",
                "ALTER TABLE [Users] NOCHECK CONSTRAINT ALL;"
            };

            if (!roleExists)
            {
                sqlParts.Add($@"
                    SET IDENTITY_INSERT [Roles] ON;
                    INSERT INTO [Roles] ([RoleId], [RoleName], [Description], [UserId], [IsActive], [CreatedAt], [ModifiedAt], [CreatedBy], [ModifiedBy])
                    VALUES ({BootstrapRoleId}, N'SuperAdmin', N'System super administrator with unrestricted access', NULL, 1, '{now:yyyy-MM-dd HH:mm:ss}', '{now:yyyy-MM-dd HH:mm:ss}', {BootstrapUserId}, {BootstrapUserId});
                    SET IDENTITY_INSERT [Roles] OFF;");
            }

            if (!userExists)
            {
                sqlParts.Add($@"
        SET IDENTITY_INSERT [Users] ON;
        INSERT INTO [Users] ([UserId], [RoleId], [FullName], [Email], [PasswordHash], [User Category], [Phone], [LastLogin], [RefreshToken], [Is2FAEnabled], [TwoFactorSecretKey], [IsActive], [CreatedAt], [ModifiedAt], [CreatedBy], [ModifiedBy])
        VALUES ({BootstrapUserId}, {BootstrapRoleId}, N'Super Admin', N'admin@fincore.com', N'{passwordHash}', N'Employee', N'9000000001', '{now:yyyy-MM-dd HH:mm:ss}', N'', 0, NULL, 1, '{now:yyyy-MM-dd HH:mm:ss}', '{now:yyyy-MM-dd HH:mm:ss}', {BootstrapUserId}, {BootstrapUserId});
        SET IDENTITY_INSERT [Users] OFF;");
            }

            sqlParts.Add("ALTER TABLE [Roles] WITH CHECK CHECK CONSTRAINT ALL;");
            sqlParts.Add("ALTER TABLE [Users] WITH CHECK CHECK CONSTRAINT ALL;");
            sqlParts.Add("COMMIT;");

            var sql = string.Join("\n", sqlParts);

            await db.Database.ExecuteSqlRawAsync(sql);
        }

        public static async Task SeedRolesAsync(AppDbContext db)
        {
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