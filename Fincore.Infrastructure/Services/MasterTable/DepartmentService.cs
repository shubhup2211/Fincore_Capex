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
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public DepartmentService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        public async Task<ApiResponse<IEnumerable<DepartmentDTO>>> GetAllDepartmentsAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Departments.CountAsync();

                var departments = await db.Departments
                    .Include(x => x.Company)
                    .Include(x => x.MasterType)
                    .Include(x => x.Manager)
                        .ThenInclude(x => x.User)
                    .OrderBy(x => x.DepartmentId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var departmentDTOs =
                    mapper.Map<IEnumerable<DepartmentDTO>>(departments);

                var metadata = new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    departmentDTOs,
                    "Departments retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<IEnumerable<DepartmentDTO>>(
                    "Failed to retrieve departments.",
                    "DEPARTMENT_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<DepartmentDTO>> GetDepartmentByIdAsync(int id)
        {
            try
            {
                var department = await db.Departments
                    .Include(x => x.Company)
                    .Include(x => x.MasterType)
                    .Include(x => x.Manager)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.DepartmentId == id);

                if (department == null)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Department not found.",
                        "DEPARTMENT_NOT_FOUND",
                        $"Department with ID {id} does not exist.");
                }

                var departmentDTO =
                    mapper.Map<DepartmentDTO>(department);

                return ApiResponseHelper.SuccessRes(
                    departmentDTO,
                    "Department retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<DepartmentDTO>(
                    "Failed to retrieve department.",
                    "DEPARTMENT_GET_ERROR",
                    ex.Message);
            }
        }


        public async Task<ApiResponse<DepartmentDTO>> CreateDepartmentAsync(
            DepartmentDTO departmentDTO)
        {
            try
            {
                
                var companyExists = await db.Companies
                    .AnyAsync(x => x.CompanyId == departmentDTO.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {departmentDTO.CompanyId} does not exist.");
                }


               
                var duplicate = await db.Departments
                    .AnyAsync(x =>
                        x.CompanyId == departmentDTO.CompanyId &&
                        x.DepartmentCode == departmentDTO.DepartmentCode);

                if (duplicate)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Department code already exists.",
                        "DUPLICATE_DEPARTMENT_CODE",
                        $"Department code {departmentDTO.DepartmentCode} already exists for this company.");
                }


                if (departmentDTO.MasterTypeId.HasValue)
                {
                    var masterTypeExists = await db.MasterTypes
                        .AnyAsync(x =>
                            x.MasterTypeId == departmentDTO.MasterTypeId.Value);

                    if (!masterTypeExists)
                    {
                        return ApiResponseHelper.Failure<DepartmentDTO>(
                            "Master type not found.",
                            "MASTER_TYPE_NOT_FOUND",
                            $"Master type with ID {departmentDTO.MasterTypeId} does not exist.");
                    }
                }


              
                if (departmentDTO.ManagerId.HasValue)
                {
                    var managerExists = await db.Employees
                        .AnyAsync(x =>
                            x.EmployeeId == departmentDTO.ManagerId.Value);

                    if (!managerExists)
                    {
                        return ApiResponseHelper.Failure<DepartmentDTO>(
                            "Manager not found.",
                            "MANAGER_NOT_FOUND",
                            $"Employee with ID {departmentDTO.ManagerId} does not exist.");
                    }
                }


                var department =
                    mapper.Map<Department>(departmentDTO);

                department.DepartmentId = 0;
                department.CreatedAt = DateTime.UtcNow;
                department.ModifiedAt = DateTime.UtcNow;

                await db.Departments.AddAsync(department);
                await db.SaveChangesAsync();


                
                var createdDepartment = await db.Departments
                    .Include(x => x.Company)
                    .Include(x => x.MasterType)
                    .Include(x => x.Manager)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(
                        x => x.DepartmentId == department.DepartmentId);

                var result =
                    mapper.Map<DepartmentDTO>(createdDepartment);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Department created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<DepartmentDTO>(
                    "Failed to create department.",
                    "DEPARTMENT_CREATE_ERROR",
                    ex.Message);
            }
        }


        
        public async Task<ApiResponse<DepartmentDTO>> UpdateDepartmentAsync(
            int id,
            DepartmentDTO departmentDTO)
        {
            try
            {
                var department = await db.Departments
                    .FirstOrDefaultAsync(x => x.DepartmentId == id);

                if (department == null)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Department not found.",
                        "DEPARTMENT_NOT_FOUND",
                        $"Department with ID {id} does not exist.");
                }


                var companyExists = await db.Companies
                    .AnyAsync(x => x.CompanyId == departmentDTO.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {departmentDTO.CompanyId} does not exist.");
                }


                var duplicate = await db.Departments
                    .AnyAsync(x =>
                        x.CompanyId == departmentDTO.CompanyId &&
                        x.DepartmentCode == departmentDTO.DepartmentCode &&
                        x.DepartmentId != id);

                if (duplicate)
                {
                    return ApiResponseHelper.Failure<DepartmentDTO>(
                        "Department code already exists.",
                        "DUPLICATE_DEPARTMENT_CODE",
                        $"Department code {departmentDTO.DepartmentCode} already exists for this company.");
                }


                if (departmentDTO.MasterTypeId.HasValue)
                {
                    var masterTypeExists = await db.MasterTypes
                        .AnyAsync(x =>
                            x.MasterTypeId == departmentDTO.MasterTypeId.Value);

                    if (!masterTypeExists)
                    {
                        return ApiResponseHelper.Failure<DepartmentDTO>(
                            "Master type not found.",
                            "MASTER_TYPE_NOT_FOUND",
                            $"Master type with ID {departmentDTO.MasterTypeId} does not exist.");
                    }
                }


                if (departmentDTO.ManagerId.HasValue)
                {
                    var managerExists = await db.Employees
                        .AnyAsync(x =>
                            x.EmployeeId == departmentDTO.ManagerId.Value);

                    if (!managerExists)
                    {
                        return ApiResponseHelper.Failure<DepartmentDTO>(
                            "Manager not found.",
                            "MANAGER_NOT_FOUND",
                            $"Employee with ID {departmentDTO.ManagerId} does not exist.");
                    }
                }


                department.CompanyId = departmentDTO.CompanyId;
                department.DepartmentName = departmentDTO.DepartmentName;
                department.DepartmentCode = departmentDTO.DepartmentCode;
                department.MasterTypeId = departmentDTO.MasterTypeId;
                department.ManagerId = departmentDTO.ManagerId;
                department.IsActive = departmentDTO.IsActive;

                department.ModifiedBy = departmentDTO.ModifiedBy;
                department.ModifiedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();


                var updatedDepartment = await db.Departments
                    .Include(x => x.Company)
                    .Include(x => x.MasterType)
                    .Include(x => x.Manager)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.DepartmentId == id);

                var result =
                    mapper.Map<DepartmentDTO>(updatedDepartment);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Department updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<DepartmentDTO>(
                    "Failed to update department.",
                    "DEPARTMENT_UPDATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await db.Departments
                    .FirstOrDefaultAsync(x => x.DepartmentId == id);

                if (department == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Department not found.",
                        "DEPARTMENT_NOT_FOUND",
                        $"Department with ID {id} does not exist.");
                }

                db.Departments.Remove(department);

                await db.SaveChangesAsync();

                return ApiResponseHelper.SuccessRes(
                    true,
                    "Department deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete department.",
                    "DEPARTMENT_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}