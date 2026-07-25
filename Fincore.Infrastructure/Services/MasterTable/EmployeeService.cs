using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.MasterTable
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public EmployeeService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Employees.CountAsync();

                var employees = await db.Employees
                    .Include(x => x.User)
                    .Include(x => x.Department)
                    .Include(x => x.DesignationRole)
                    .Include(x => x.Company)
                    .Include(x => x.ReportingManagerEmployee)
                        .ThenInclude(x => x.User)
                    .OrderBy(x => x.EmployeeId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var employeeDtos =
                    mapper.Map<List<EmployeeDto>>(employees);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    employeeDtos,
                    "Employees retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<EmployeeDto>>(
                    "Failed to retrieve employees.",
                    "EMPLOYEE_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(
            int id)
        {
            try
            {
                var employee = await db.Employees
                    .Include(x => x.User)
                    .Include(x => x.Department)
                    .Include(x => x.DesignationRole)
                    .Include(x => x.Company)
                    .Include(x => x.ReportingManagerEmployee)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(
                        x => x.EmployeeId == id);

                if (employee == null)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Employee not found.",
                        "EMPLOYEE_NOT_FOUND",
                        $"Employee with ID {id} does not exist.");
                }

                var employeeDto =
                    mapper.Map<EmployeeDto>(employee);

                return ApiResponseHelper.SuccessRes(
                    employeeDto,
                    "Employee retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<EmployeeDto>(
                    "Failed to retrieve employee.",
                    "EMPLOYEE_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(
            CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                var employeeCodeExists = await db.Employees
                    .AnyAsync(x =>
                        x.EmployeeCode == createEmployeeDto.EmployeeCode);

                if (employeeCodeExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Employee code already exists.",
                        "DUPLICATE_EMPLOYEE_CODE",
                        $"Employee code {createEmployeeDto.EmployeeCode} already exists.");
                }

                var userExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == createEmployeeDto.UserId);

                if (!userExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "User not found.",
                        "USER_NOT_FOUND",
                        $"User with ID {createEmployeeDto.UserId} does not exist.");
                }

                var userEmployeeExists = await db.Employees
                    .AnyAsync(x =>
                        x.UserId == createEmployeeDto.UserId);

                if (userEmployeeExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "User is already assigned to an employee.",
                        "DUPLICATE_EMPLOYEE_USER",
                        $"User with ID {createEmployeeDto.UserId} is already linked to an employee.");
                }


                var department = await db.Departments
                    .FirstOrDefaultAsync(x =>
                        x.DepartmentId ==
                        createEmployeeDto.DepartmentId);

                if (department == null)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Department not found.",
                        "DEPARTMENT_NOT_FOUND",
                        $"Department with ID {createEmployeeDto.DepartmentId} does not exist.");
                }

                var designationExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleId == createEmployeeDto.Designation);

                if (!designationExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Designation role not found.",
                        "DESIGNATION_NOT_FOUND",
                        $"Role with ID {createEmployeeDto.Designation} does not exist.");
                }

                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId == createEmployeeDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {createEmployeeDto.CompanyId} does not exist.");
                }

                if (department.CompanyId != createEmployeeDto.CompanyId)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Department does not belong to the selected company.",
                        "DEPARTMENT_COMPANY_MISMATCH",
                        $"Department with ID {createEmployeeDto.DepartmentId} does not belong to company with ID {createEmployeeDto.CompanyId}.");
                }

                if (createEmployeeDto.ReportingManager.HasValue)
                {
                    var reportingManager = await db.Employees
                        .FirstOrDefaultAsync(x =>
                            x.EmployeeId ==
                            createEmployeeDto.ReportingManager.Value);

                    if (reportingManager == null)
                    {
                        return ApiResponseHelper.Failure<EmployeeDto>(
                            "Reporting manager not found.",
                            "REPORTING_MANAGER_NOT_FOUND",
                            $"Employee with ID {createEmployeeDto.ReportingManager.Value} does not exist.");
                    }

                    if (reportingManager.CompanyId !=
                        createEmployeeDto.CompanyId)
                    {
                        return ApiResponseHelper.Failure<EmployeeDto>(
                            "Reporting manager belongs to another company.",
                            "REPORTING_MANAGER_COMPANY_MISMATCH",
                            "Reporting manager must belong to the same company as the employee.");
                    }
                }

                var employee =
                    mapper.Map<Employee>(createEmployeeDto);

                employee.EmployeeId = 0;

                await db.Employees.AddAsync(employee);
                await db.SaveChangesAsync();

                var createdEmployee = await db.Employees
                    .Include(x => x.User)
                    .Include(x => x.Department)
                    .Include(x => x.DesignationRole)
                    .Include(x => x.Company)
                    .Include(x => x.ReportingManagerEmployee)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(
                        x => x.EmployeeId == employee.EmployeeId);

                var result =
                    mapper.Map<EmployeeDto>(createdEmployee);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Employee created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<EmployeeDto>(
                    "Failed to create employee.",
                    "EMPLOYEE_CREATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(
            int id,
            UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var employee = await db.Employees
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == id);

                if (employee == null)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Employee not found.",
                        "EMPLOYEE_NOT_FOUND",
                        $"Employee with ID {id} does not exist.");
                }

                var employeeCodeExists = await db.Employees
                    .AnyAsync(x =>
                        x.EmployeeCode ==
                            updateEmployeeDto.EmployeeCode &&
                        x.EmployeeId != id);

                if (employeeCodeExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Employee code already exists.",
                        "DUPLICATE_EMPLOYEE_CODE",
                        $"Employee code {updateEmployeeDto.EmployeeCode} already exists.");
                }

                var userExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == updateEmployeeDto.UserId);

                if (!userExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "User not found.",
                        "USER_NOT_FOUND",
                        $"User with ID {updateEmployeeDto.UserId} does not exist.");
                }

                var userEmployeeExists = await db.Employees
                    .AnyAsync(x =>
                        x.UserId == updateEmployeeDto.UserId &&
                        x.EmployeeId != id);

                if (userEmployeeExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "User is already assigned to another employee.",
                        "DUPLICATE_EMPLOYEE_USER",
                        $"User with ID {updateEmployeeDto.UserId} is already linked to another employee.");
                }

                var department = await db.Departments
                    .FirstOrDefaultAsync(x =>
                        x.DepartmentId ==
                        updateEmployeeDto.DepartmentId);

                if (department == null)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Department not found.",
                        "DEPARTMENT_NOT_FOUND",
                        $"Department with ID {updateEmployeeDto.DepartmentId} does not exist.");
                }

                var designationExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleId == updateEmployeeDto.Designation);

                if (!designationExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Designation role not found.",
                        "DESIGNATION_NOT_FOUND",
                        $"Role with ID {updateEmployeeDto.Designation} does not exist.");
                }

                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId == updateEmployeeDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {updateEmployeeDto.CompanyId} does not exist.");
                }

                if (department.CompanyId != updateEmployeeDto.CompanyId)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Department does not belong to the selected company.",
                        "DEPARTMENT_COMPANY_MISMATCH",
                        $"Department with ID {updateEmployeeDto.DepartmentId} does not belong to company with ID {updateEmployeeDto.CompanyId}.");
                }

                if (updateEmployeeDto.ReportingManager.HasValue &&
                    updateEmployeeDto.ReportingManager.Value == id)
                {
                    return ApiResponseHelper.Failure<EmployeeDto>(
                        "Employee cannot be their own reporting manager.",
                        "INVALID_REPORTING_MANAGER",
                        $"Employee with ID {id} cannot report to themselves.");
                }

                if (updateEmployeeDto.ReportingManager.HasValue)
                {
                    var reportingManager = await db.Employees
                        .FirstOrDefaultAsync(x =>
                            x.EmployeeId ==
                            updateEmployeeDto.ReportingManager.Value);

                    if (reportingManager == null)
                    {
                        return ApiResponseHelper.Failure<EmployeeDto>(
                            "Reporting manager not found.",
                            "REPORTING_MANAGER_NOT_FOUND",
                            $"Employee with ID {updateEmployeeDto.ReportingManager.Value} does not exist.");
                    }

                    if (reportingManager.CompanyId !=
                        updateEmployeeDto.CompanyId)
                    {
                        return ApiResponseHelper.Failure<EmployeeDto>(
                            "Reporting manager belongs to another company.",
                            "REPORTING_MANAGER_COMPANY_MISMATCH",
                            "Reporting manager must belong to the same company as the employee.");
                    }
                }

                employee.EmployeeCode =
                    updateEmployeeDto.EmployeeCode;

                employee.UserId =
                    updateEmployeeDto.UserId;

                employee.DepartmentId =
                    updateEmployeeDto.DepartmentId;

                employee.Designation =
                    updateEmployeeDto.Designation;

                employee.JoiningDate =
                    updateEmployeeDto.JoiningDate;

                employee.CompanyId =
                    updateEmployeeDto.CompanyId;

                employee.ReportingManager =
                    updateEmployeeDto.ReportingManager;

                employee.PAN =
                    updateEmployeeDto.PAN;

                employee.IsActive =
                    updateEmployeeDto.IsActive;


                await db.SaveChangesAsync();

                var updatedEmployee = await db.Employees
                    .Include(x => x.User)
                    .Include(x => x.Department)
                    .Include(x => x.DesignationRole)
                    .Include(x => x.Company)
                    .Include(x => x.ReportingManagerEmployee)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(
                        x => x.EmployeeId == id);

                var result =
                    mapper.Map<EmployeeDto>(updatedEmployee);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<EmployeeDto>(
                    "Failed to update employee.",
                    "EMPLOYEE_UPDATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await db.Employees
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == id);

                if (employee == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Employee not found.",
                        "EMPLOYEE_NOT_FOUND",
                        $"Employee with ID {id} does not exist.");
                }

                var hasSubordinates = await db.Employees
                    .AnyAsync(x =>
                        x.ReportingManager == id);

                if (hasSubordinates)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Employee cannot be deleted.",
                        "EMPLOYEE_HAS_SUBORDINATES",
                        "This employee is currently assigned as a reporting manager.");
                }

                db.Employees.Remove(employee);

                await db.SaveChangesAsync();

                return ApiResponseHelper.SuccessRes(
                    true,
                    "Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete employee.",
                    "EMPLOYEE_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}