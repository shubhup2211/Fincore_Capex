using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Constants
{
    public class RoleConstants
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string FinanceManager = "Finance Manager";
        public const string ProcurementHead = "Procurement Head";
        public const string HRManager = "HR Manager";
        public const string DepartmentHead = "Department Head";
        public const string Employee = "Employee";
        public const string Vendor = "Vendor";
        public const string Customer = "Customer";
        public const string Accountant = "Accountant";
        public const string Auditor = "Auditor";

        // Common combinations reusable across modules
        public const string AdminOnly = SuperAdmin + "," + Admin;
        public const string FinanceTeam = SuperAdmin + "," + Admin + "," + FinanceManager + "," + Accountant;
        public const string ProcurementTeam = SuperAdmin + "," + Admin + "," + ProcurementHead;
        public const string ManagementTeam = SuperAdmin + "," + Admin + "," + DepartmentHead + "," + FinanceManager;
    }
}
