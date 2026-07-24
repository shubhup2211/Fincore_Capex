namespace Fincore.Application.DTO.MasterTable
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }

        // User
        public int UserId { get; set; }
        public string UserName { get; set; }

        // Department
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        // Designation / Role
        public int Designation { get; set; }
        public string DesignationName { get; set; }

        public DateTime? JoiningDate { get; set; }

        // Company
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        // Reporting Manager
        public int? ReportingManager { get; set; }
        public string? ReportingManagerName { get; set; }

        public string PAN { get; set; }

        public byte IsActive { get; set; }
    }
}