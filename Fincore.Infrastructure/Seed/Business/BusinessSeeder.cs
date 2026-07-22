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

namespace Fincore.Infrastructure.Seed.Business
{
    /// <summary>Phase 4 – Customer, VendorCategory, Vendor, DocumentType, Document</summary>
    public static class BusinessSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedCustomersAsync(db);
            await SeedVendorCategoriesAsync(db);
            await SeedVendorsAsync(db);
            await SeedDocumentTypesAsync(db);
            await SeedDocumentsAsync(db);
        }

        private static async Task SeedCustomersAsync(AppDbContext db)
        {
            if (await db.Customers.AnyAsync()) return;

            var customerUsers = await db.Users.Where(u => u.UserCategory == "Customer").OrderBy(u => u.UserId).ToListAsync();
            var companies = await db.Companies.OrderBy(c => c.CompanyId).ToListAsync();

            var list = new List<Customer>();
            for (int i = 0; i < customerUsers.Count; i++)
            {
                var comp = companies[i % companies.Count];
                list.Add(new Customer
                {
                    CustomerCode = $"CUST-{comp.CompanyCode}-{(i + 1):D3}",
                    UserId       = customerUsers[i].UserId,
                    CompanyId    = comp.CompanyId,
                    IsActive     = 1
                });
            }
            db.Customers.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedVendorCategoriesAsync(AppDbContext db)
        {
            if (await db.VendorCategories.AnyAsync()) return;

            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;
            var names = new[]
            {
                ("IT Hardware",         "Servers, laptops, networking gear"),
                ("Software Services",   "Licences, SaaS subscriptions, dev services"),
                ("Office Supplies",     "Stationery, furniture, pantry"),
                ("Facilities Mgmt",     "Housekeeping, security, maintenance"),
                ("Logistics & Freight", "Courier, freight, warehousing"),
                ("Marketing & Media",   "Ad-agencies, printing, events"),
                ("Utilities",           "Electricity, water, telecom"),
                ("Professional Svcs",   "Legal, audit, consulting"),
                ("Raw Materials",       "Manufacturing inputs & consumables"),
                ("Travel & Hospitality","Air travel, hotels, ground transport")
            };
            var list = names.Select(n => new VendorCategory
            {
                CategoryName = n.Item1,
                Description  = n.Item2,
                IsActive     = 1,
                CreatedAt    = now,
                ModifiedAt   = now,
                CreatedBy    = admin,
                ModifiedBy   = admin
            }).ToList();
            db.VendorCategories.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedVendorsAsync(AppDbContext db)
        {
            if (await db.Vendors.AnyAsync()) return;

            var vendorUsers = await db.Users.Where(u => u.UserCategory == "Vendor").OrderBy(u => u.UserId).ToListAsync();
            var categories  = await db.VendorCategories.OrderBy(v => v.VendorCategoryId).ToListAsync();
            var companies   = await db.Companies.OrderBy(c => c.CompanyId).ToListAsync();
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;
            var rng = new Randomizer(FakerHelper.GlobalSeed + 4);

            var vendorNames = new[]
            {
                "Steelworks Ltd.","BluePrint Systems","PrimeCargo Logistics","NexaOffice Supplies","GreenPower Utilities",
                "Zenith IT Hardware","Nimbus SaaS Cloud","OrbitCourier Express","Skyline Facilities","Momentum Legal LLP",
                "Delta Print Media","AeroFuel Suppliers","Vertex Consulting","Fusion Rawmat","Beacon Travel Services",
                "Cirrus Networks","Argon Marketing","Lumen Media","Kinetic Freight","Sable Consultants",
                "Titan Utilities","Halcyon Software","Northwind Supplies","Pinnacle Servers","Everest Housekeeping"
            };

            var list = new List<Vendor>();
            for (int i = 0; i < vendorNames.Length; i++)
            {
                var comp = companies[i % companies.Count];
                var cat  = categories[i % categories.Count];
                list.Add(new Vendor
                {
                    VendorCode        = $"VND-{comp.CompanyCode}-{(i + 1):D3}",
                    VendorCategoryId  = cat.VendorCategoryId,
                    CompanyId         = comp.CompanyId,
                    BankAccount       = rng.Long(10000000000, 99999999999).ToString(),
                    PAN               = $"BGHIJ{rng.Int(1000, 9999)}K",
                    PerformanceScore  = System.Math.Round((decimal)rng.Double(3.2, 4.9), 2),
                    IsVerified        = (byte)(rng.Bool() ? 1 : 0),
                    IsActive          = 1,
                    CreatedAt         = now,
                    ModifiedAt        = now,
                    CreatedBy         = admin,
                    ModifiedBy        = admin
                });
            }
            db.Vendors.AddRange(list);
            await db.SaveChangesAsync();

            // OPTIONAL – link the first 5 vendor users to first 5 vendors (informational only – no FK)
            // Vendor entity does not carry UserId, so nothing further to update.
            _ = vendorUsers;
        }

        private static async Task SeedDocumentTypesAsync(AppDbContext db)
        {
            if (await db.DocumentTypes.AnyAsync()) return;
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;
            var cats = new[] { "KYC", "Contract", "Invoice", "Receipt", "PO", "Compliance", "Tax", "Bank" };
            db.DocumentTypes.AddRange(cats.Select(c => new DocumentType
            {
                DocumentCategory = c,
                IsActive = 1,
                CreatedAt = now,
                ModifiedAt = now,
                CreatedBy = admin,
                ModifiedBy = admin
            }));
            await db.SaveChangesAsync();
        }

        private static async Task SeedDocumentsAsync(AppDbContext db)
        {
            if (await db.Documents.AnyAsync()) return;

            var types = await db.DocumentTypes.ToListAsync();
            var users = await db.Users.OrderBy(u => u.UserId).ToListAsync();
            var masterTypes = await db.MasterTypes.ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 5);
            var now = DateTime.UtcNow;

            var fileTypes = new[] { "pdf", "docx", "xlsx", "png", "jpg" };
            var list = new List<Document>();
            for (int i = 0; i < 25; i++)
            {
                var dt   = types[i % types.Count];
                var user = users[i % users.Count];
                var mt   = masterTypes[i % masterTypes.Count];
                var ext  = fileTypes[i % fileTypes.Length];
                list.Add(new Document
                {
                    DocumentTypeId = dt.DocumentTypeId,
                    UserId         = user.UserId,
                    EntityId       = i + 1,
                    MasterTypeId   = mt.MasterTypeId,
                    FileName       = $"{dt.DocumentCategory}_Doc_{i + 1:D3}.{ext}",
                    FilePath       = $"/uploads/{dt.DocumentCategory.ToLower()}/doc_{i + 1:D3}.{ext}",
                    FileType       = ext,
                    CreatedAt      = now.AddDays(-rng.Int(1, 400)),
                    ModifiedAt     = now
                });
            }
            db.Documents.AddRange(list);
            await db.SaveChangesAsync();
        }
    }
}
