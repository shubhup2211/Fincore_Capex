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

namespace Fincore.Infrastructure.Seed.Organization
{
    /// <summary>Phase 3 – Company ➜ Department ➜ Employee</summary>
    public static class OrganizationSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedCompaniesAsync(db);
            await SeedDepartmentsAsync(db);
            await SeedEmployeesAsync(db);
        }

        private static async Task SeedCompaniesAsync(AppDbContext db)
        {
            if (await db.Companies.AnyAsync()) return;

            var countries = await db.Countries.ToListAsync();
            var mt = await db.MasterTypes.FirstAsync(m => m.MasterTypeName == "Company");
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;

            int C(string n) => countries.First(c => c.CountryName == n).CountryId;

            var companies = new List<Company>
            {
                new() { CompanyCode = "FCR-IN",   CompanyName = "Fincore India Pvt Ltd",      CountryId = C("India"),          ContactNumber = "+91-2261234501", ContactEmail = "contact.in@fincore.com",  GSTIN = "27AAACF1234M1Z5", CIN = "U72900MH2018PTC1", PAN = "AAACF1234M", TAN = "MUMR11111A", Address = "5th Floor, Andheri East, Mumbai" },
                new() { CompanyCode = "FCR-US",   CompanyName = "Fincore USA LLC",            CountryId = C("United States"),  ContactNumber = "+1-212-555-0102", ContactEmail = "contact.us@fincore.com",  GSTIN = "US-EIN-113",     CIN = "US-DE-2019-01", PAN = "US-EIN-1130", TAN = "-",           Address = "500 Madison Ave, New York" },
                new() { CompanyCode = "FCR-UK",   CompanyName = "Fincore UK Limited",         CountryId = C("United Kingdom"), ContactNumber = "+44-20-7946-0003", ContactEmail = "contact.uk@fincore.com",  GSTIN = "GB123456789",    CIN = "UK-LDN-2020-03", PAN = "GB-CTX-3030", TAN = "-",         Address = "10 Finsbury Square, London" },
                new() { CompanyCode = "FCR-SG",   CompanyName = "Fincore Singapore Pte",      CountryId = C("Singapore"),      ContactNumber = "+65-6555-0104", ContactEmail = "contact.sg@fincore.com",  GSTIN = "SG-GST-4040",    CIN = "SG-ACRA-2021-04", PAN = "SG-CTX-4040", TAN = "-",       Address = "Marina Bay Financial Ctr, Singapore" },
                new() { CompanyCode = "FCR-AE",   CompanyName = "Fincore Middle East FZC",    CountryId = C("UAE"),            ContactNumber = "+971-4-555-0105", ContactEmail = "contact.ae@fincore.com",  GSTIN = "AE-TRN-5050",    CIN = "AE-DXB-2022-05", PAN = "AE-CTX-5050", TAN = "-",         Address = "Emaar Sq, Downtown Dubai" }
            };

            foreach (var c in companies)
            {
                c.MasterTypeId = mt.MasterTypeId;
                c.IsActive     = 1;
                c.CreatedAt    = now;
                c.ModifiedAt   = now;
                c.CreatedBy    = admin;
                c.ModifiedBy   = admin;
            }
            db.Companies.AddRange(companies);
            await db.SaveChangesAsync();
        }

        private static async Task SeedDepartmentsAsync(AppDbContext db)
        {
            if (await db.Departments.AnyAsync()) return;

            var companies = await db.Companies.OrderBy(c => c.CompanyId).ToListAsync();
            var mt = await db.MasterTypes.FirstAsync(m => m.MasterTypeName == "Company");
            var admin = RoleSeeder.BootstrapUserId;
            var now = DateTime.UtcNow;

            var depNames = new[]
            {
                "Finance","HR","Procurement","IT","Sales","Marketing","Operations","Admin","R&D","Legal","Logistics","Support","Manufacturing","Quality","CustomerSuccess"
            };

            var list = new List<Department>();
            int idx = 0;
            foreach (var comp in companies)
            {
                // 3 departments per company => 15 total
                for (int j = 0; j < 3 && idx < depNames.Length; j++, idx++)
                {
                    list.Add(new Department
                    {
                        CompanyId      = comp.CompanyId,
                        DepartmentName = depNames[idx],
                        DepartmentCode = $"{comp.CompanyCode}-{depNames[idx].Substring(0, System.Math.Min(3, depNames[idx].Length)).ToUpper()}",
                        MasterTypeId   = mt.MasterTypeId,
                        ManagerId      = null, // resolved later after employees exist
                        IsActive       = 1,
                        CreatedAt      = now,
                        ModifiedAt     = now,
                        CreatedBy      = admin,
                        ModifiedBy     = admin
                    });
                }
            }
            db.Departments.AddRange(list);
            await db.SaveChangesAsync();
        }

        private static async Task SeedEmployeesAsync(AppDbContext db)
        {
            if (await db.Employees.AnyAsync()) return;

            var users = await db.Users
                .Where(u => u.UserCategory == "Employee")
                .OrderBy(u => u.UserId)
                .ToListAsync();
            var departments = await db.Departments.OrderBy(d => d.DepartmentId).ToListAsync();
            var companies   = await db.Companies.OrderBy(c => c.CompanyId).ToListAsync();
            var roles       = await db.Roles.ToListAsync();
            var rng = new Randomizer(FakerHelper.GlobalSeed + 3);

            int R(string n) => roles.First(r => r.RoleName == n).RoleId;

            var employees = new List<Employee>();
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var dept = departments[i % departments.Count];
                var company = companies.First(c => c.CompanyId == dept.CompanyId);

                string designation;
                if (user.RoleId == R("Admin") || user.RoleId == R("HR Manager") || user.RoleId == R("Finance Manager") || user.RoleId == R("Procurement Head"))
                    designation = user.RoleId.ToString();
                else if (user.RoleId == R("Department Head"))
                    designation = R("Department Head").ToString();
                else
                    designation = R("Employee").ToString();

                employees.Add(new Employee
                {
                    EmployeeCode      = $"{company.CompanyCode}-EMP-{(i + 1):D4}",
                    UserId            = user.UserId,
                    DepartmentId      = dept.DepartmentId,
                    Designation       = user.RoleId,     // reuse the user's role as designation
                    JoiningDate       = DateTime.UtcNow.AddDays(-rng.Int(60, 1800)),
                    CompanyId         = company.CompanyId,
                    ReportingManager  = null,
                    PAN               = $"ABCDE{rng.Int(1000, 9999)}F",
                    IsActive          = 1
                });
            }
            db.Employees.AddRange(employees);
            await db.SaveChangesAsync();

            // Post-process: assign reporting managers + department managers
            var saved = await db.Employees.OrderBy(e => e.EmployeeId).ToListAsync();
            // First employee in each department becomes manager for that dept
            var groupedByDept = saved.GroupBy(e => e.DepartmentId).ToList();
            foreach (var g in groupedByDept)
            {
                var head = g.First();
                var dept = await db.Departments.FirstAsync(d => d.DepartmentId == g.Key);
                dept.ManagerId = head.EmployeeId;
                foreach (var e in g.Skip(1))
                    e.ReportingManager = head.EmployeeId;
            }
            await db.SaveChangesAsync();
        }
    }
}
