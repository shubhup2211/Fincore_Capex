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
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public PermissionService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<PermissionDto>>>
            GetAllPermissionsAsync(
                int pageNumber,
                int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Permissions.CountAsync();

                var permissions = await db.Permissions
                    .Include(x => x.Role)
                    .Include(x => x.MasterType)
                    .OrderBy(x => x.PermissionId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var permissionDtos =
                    mapper.Map<List<PermissionDto>>(permissions);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    permissionDtos,
                    "Permissions retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<PermissionDto>>(
                    "Failed to retrieve permissions.",
                    "PERMISSION_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<PermissionDto>>
            GetPermissionByIdAsync(int id)
        {
            try
            {
                var permission = await db.Permissions
                    .Include(x => x.Role)
                    .Include(x => x.MasterType)
                    .FirstOrDefaultAsync(
                        x => x.PermissionId == id);

                if (permission == null)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Permission not found.",
                        "PERMISSION_NOT_FOUND",
                        $"Permission with ID {id} does not exist.");
                }

                var permissionDto =
                    mapper.Map<PermissionDto>(permission);

                return ApiResponseHelper.SuccessRes(
                    permissionDto,
                    "Permission retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<PermissionDto>(
                    "Failed to retrieve permission.",
                    "PERMISSION_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<PermissionDto>>
            CreatePermissionAsync(
                CreatePermissionDto createPermissionDto)
        {
            try
            {
                var roleExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleId == createPermissionDto.RoleId);

                if (!roleExists)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Role not found.",
                        "ROLE_NOT_FOUND",
                        $"Role with ID {createPermissionDto.RoleId} does not exist.");
                }

                if (createPermissionDto.MasterTypeId.HasValue)
                {
                    var masterTypeExists = await db.MasterTypes
                        .AnyAsync(x =>
                            x.MasterTypeId ==
                            createPermissionDto.MasterTypeId.Value);

                    if (!masterTypeExists)
                    {
                        return ApiResponseHelper.Failure<PermissionDto>(
                            "Master type not found.",
                            "MASTER_TYPE_NOT_FOUND",
                            $"Master type with ID {createPermissionDto.MasterTypeId.Value} does not exist.");
                    }
                }

                var createdByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == createPermissionDto.CreatedBy);

                if (!createdByExists)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Created by user not found.",
                        "CREATED_BY_USER_NOT_FOUND",
                        $"User with ID {createPermissionDto.CreatedBy} does not exist.");
                }


                var permission =
                    mapper.Map<Permission>(createPermissionDto);

                permission.PermissionId = 0;

                permission.CreatedAt = DateTime.UtcNow;
                permission.ModifiedAt = DateTime.UtcNow;
                permission.ModifiedBy =
                    createPermissionDto.CreatedBy;

                await db.Permissions.AddAsync(permission);
                await db.SaveChangesAsync();


                var createdPermission = await db.Permissions
                    .Include(x => x.Role)
                    .Include(x => x.MasterType)
                    .FirstOrDefaultAsync(
                        x => x.PermissionId ==
                        permission.PermissionId);

                var result =
                    mapper.Map<PermissionDto>(createdPermission);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Permission created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<PermissionDto>(
                    "Failed to create permission.",
                    "PERMISSION_CREATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<PermissionDto>>
            UpdatePermissionAsync(
                int id,
                UpdatePermissionDto updatePermissionDto)
        {
            try
            {
                var permission = await db.Permissions
                    .FirstOrDefaultAsync(
                        x => x.PermissionId == id);

                if (permission == null)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Permission not found.",
                        "PERMISSION_NOT_FOUND",
                        $"Permission with ID {id} does not exist.");
                }

                var roleExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleId == updatePermissionDto.RoleId);

                if (!roleExists)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Role not found.",
                        "ROLE_NOT_FOUND",
                        $"Role with ID {updatePermissionDto.RoleId} does not exist.");
                }

                if (updatePermissionDto.MasterTypeId.HasValue)
                {
                    var masterTypeExists = await db.MasterTypes
                        .AnyAsync(x =>
                            x.MasterTypeId ==
                            updatePermissionDto.MasterTypeId.Value);

                    if (!masterTypeExists)
                    {
                        return ApiResponseHelper.Failure<PermissionDto>(
                            "Master type not found.",
                            "MASTER_TYPE_NOT_FOUND",
                            $"Master type with ID {updatePermissionDto.MasterTypeId.Value} does not exist.");
                    }
                }

                var modifiedByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId ==
                        updatePermissionDto.ModifiedBy);

                if (!modifiedByExists)
                {
                    return ApiResponseHelper.Failure<PermissionDto>(
                        "Modified by user not found.",
                        "MODIFIED_BY_USER_NOT_FOUND",
                        $"User with ID {updatePermissionDto.ModifiedBy} does not exist.");
                }


                permission.PermissionName =
                    updatePermissionDto.PermissionName;

                permission.RoleId =
                    updatePermissionDto.RoleId;

                permission.MasterTypeId =
                    updatePermissionDto.MasterTypeId;

                permission.IsActive =
                    updatePermissionDto.IsActive;

                permission.ModifiedBy =
                    updatePermissionDto.ModifiedBy;

                permission.ModifiedAt =
                    DateTime.UtcNow;


                await db.SaveChangesAsync();


                var updatedPermission = await db.Permissions
                    .Include(x => x.Role)
                    .Include(x => x.MasterType)
                    .FirstOrDefaultAsync(
                        x => x.PermissionId == id);

                var result =
                    mapper.Map<PermissionDto>(updatedPermission);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Permission updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<PermissionDto>(
                    "Failed to update permission.",
                    "PERMISSION_UPDATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<bool>>
            DeletePermissionAsync(int id)
        {
            try
            {
                var permission = await db.Permissions
                    .FirstOrDefaultAsync(
                        x => x.PermissionId == id);

                if (permission == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Permission not found.",
                        "PERMISSION_NOT_FOUND",
                        $"Permission with ID {id} does not exist.");
                }

                db.Permissions.Remove(permission);

                await db.SaveChangesAsync();

                return ApiResponseHelper.SuccessRes(
                    true,
                    "Permission deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete permission.",
                    "PERMISSION_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}