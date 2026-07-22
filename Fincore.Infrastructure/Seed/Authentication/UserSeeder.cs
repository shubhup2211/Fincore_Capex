using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Authentication
{
    /// <summary>
    /// Phase 2 – seeds business users after the SuperAdmin bootstrap.
    /// </summary>
    public static class UserSeeder
    {
        public const string DefaultPassword = "Admin@123";

        public static async Task SeedAsync(AppDbContext db)
        {
            if (await db.Users.CountAsync() > 1) return;

            var roles = await db.Roles.ToListAsync();
            int R(string name) => roles.First(r => r.RoleName == name).RoleId;

            var now = DateTime.UtcNow;
            var users = new List<User>();

            // Deterministic Faker
            var faker = new Faker("en") { Random = new Randomizer(FakerHelper.GlobalSeed + 1) };

            (string full, string email, string role, string cat)[] fixedSet = new[]
            {
                ("Anita Sharma",      "anita.sharma@fincore.com",     "Admin",            "Employee"),
                ("Rahul Verma",       "rahul.verma@fincore.com",      "Finance Manager",  "Employee"),
                ("Priya Iyer",        "priya.iyer@fincore.com",       "Procurement Head", "Employee"),
                ("Vikram Nair",       "vikram.nair@fincore.com",      "HR Manager",       "Employee"),
                ("Sneha Kulkarni",    "sneha.kulkarni@fincore.com",   "Department Head",  "Employee"),
                ("Arjun Mehta",       "arjun.mehta@fincore.com",      "Department Head",  "Employee"),
                ("Kavya Reddy",       "kavya.reddy@fincore.com",      "Department Head",  "Employee"),
                ("Rohan Desai",       "rohan.desai@fincore.com",      "Accountant",       "Employee"),
                ("Meera Joshi",       "meera.joshi@fincore.com",      "Accountant",       "Employee"),
                ("Karan Kapoor",      "karan.kapoor@fincore.com",     "Auditor",          "Employee"),
                ("Divya Menon",       "divya.menon@fincore.com",      "Employee",         "Employee"),
                ("Aditya Rao",        "aditya.rao@fincore.com",       "Employee",         "Employee"),
                ("Neha Patel",        "neha.patel@fincore.com",       "Employee",         "Employee"),
                ("Sameer Khan",       "sameer.khan@fincore.com",      "Employee",         "Employee"),
                ("Ishita Chatterjee", "ishita.c@fincore.com",         "Employee",         "Employee"),
                ("Manoj Pillai",      "manoj.pillai@fincore.com",     "Employee",         "Employee"),
                ("Pooja Bansal",      "pooja.bansal@fincore.com",     "Employee",         "Employee"),
                ("Nikhil Saxena",     "nikhil.saxena@fincore.com",    "Employee",         "Employee"),
                // Vendor users
                ("Steelworks Ltd.",     "contact@steelworks.io",      "Vendor", "Vendor"),
                ("BluePrint Systems",   "sales@blueprintsys.com",     "Vendor", "Vendor"),
                ("PrimeCargo Logistics","ops@primecargo.io",          "Vendor", "Vendor"),
                ("NexaOffice Supplies", "hello@nexaoffice.com",       "Vendor", "Vendor"),
                ("GreenPower Utilities","desk@greenpower.io",         "Vendor", "Vendor"),
                // Customer users
                ("Orion Retailers",     "billing@orionretail.com",    "Customer", "Customer"),
                ("Titan Industries",    "ap@titan-industries.com",    "Customer", "Customer"),
                ("Zephyr Traders",      "finance@zephyrtraders.io",   "Customer", "Customer"),
                ("Halo Consumer Corp.", "invoices@halocorp.com",      "Customer", "Customer"),
                ("Vega Wholesale",      "accounts@vegawholesale.io",  "Customer", "Customer"),
                ("Aurora Distributors", "ar@aurora-dist.com",         "Customer", "Customer"),
                ("Kestrel Holdings",    "pay@kestrelhold.io",         "Customer", "Customer")
            };

            foreach (var row in fixedSet)
            {
                users.Add(new User
                {
                    RoleId       = R(row.role),
                    FullName     = row.full,
                    Email        = row.email,
                    PasswordHash = DefaultPassword,
                    UserCategory = row.cat,
                    Phone        = faker.Phone.PhoneNumber("##########").Substring(0, 10),
                    LastLogin = now,
                    RefreshToken = string.Empty,
                    IsActive     = 1,                   
                    CreatedAt    = now,
                    ModifiedAt   = now,
                    CreatedBy    = RoleSeeder.BootstrapUserId,
                    ModifiedBy   = RoleSeeder.BootstrapUserId
                });
            }

            db.Users.AddRange(users);
            await db.SaveChangesAsync();
        }
    }
}
