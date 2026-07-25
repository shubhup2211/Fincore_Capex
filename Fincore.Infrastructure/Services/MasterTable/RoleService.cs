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
    public class RoleService : IRoleService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public RoleService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<RoleDto>>> GetAllRolesAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Roles.CountAsync();

                var roles = await db.Roles
                    .OrderBy(x => x.RoleId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var roleDtos = mapper.Map<List<RoleDto>>(roles);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    roleDtos,
                    "Roles retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<RoleDto>>(
                    "Failed to retrieve roles.",
                    "ROLE_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<RoleDto>> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await db.Roles
                    .FirstOrDefaultAsync(x => x.RoleId == id);

                if (role == null)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Role not found.",
                        "ROLE_NOT_FOUND",
                        $"Role with ID {id} does not exist.");
                }

                var roleDto = mapper.Map<RoleDto>(role);

                return ApiResponseHelper.SuccessRes(
                    roleDto,
                    "Role retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<RoleDto>(
                    "Failed to retrieve role.",
                    "ROLE_GET_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<RoleDto>> CreateRoleAsync(
            CreateRoleDto createRoleDto)
        {
            try
            {
                var roleExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleName == createRoleDto.RoleName);

                if (roleExists)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Role name already exists.",
                        "DUPLICATE_ROLE_NAME",
                        $"Role with name {createRoleDto.RoleName} already exists.");
                }

                var createdByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == createRoleDto.CreatedBy);

                if (!createdByExists)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Created by user not found.",
                        "CREATED_BY_USER_NOT_FOUND",
                        $"User with ID {createRoleDto.CreatedBy} does not exist.");
                }


                var role = mapper.Map<Role>(createRoleDto);

                role.RoleId = 0;

                role.CreatedAt = DateTime.UtcNow;
                role.ModifiedAt = DateTime.UtcNow;
                role.ModifiedBy = createRoleDto.CreatedBy;


                await db.Roles.AddAsync(role);
                await db.SaveChangesAsync();


                var createdRole = await db.Roles
                    .FirstOrDefaultAsync(
                        x => x.RoleId == role.RoleId);

                var result = mapper.Map<RoleDto>(createdRole);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Role created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<RoleDto>(
                    "Failed to create role.",
                    "ROLE_CREATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(
            int id,
            UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await db.Roles
                    .FirstOrDefaultAsync(x => x.RoleId == id);

                if (role == null)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Role not found.",
                        "ROLE_NOT_FOUND",
                        $"Role with ID {id} does not exist.");
                }

                var roleNameExists = await db.Roles
                    .AnyAsync(x =>
                        x.RoleName == updateRoleDto.RoleName &&
                        x.RoleId != id);

                if (roleNameExists)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Role name already exists.",
                        "DUPLICATE_ROLE_NAME",
                        $"Role with name {updateRoleDto.RoleName} already exists.");
                }

                var modifiedByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId == updateRoleDto.ModifiedBy);

                if (!modifiedByExists)
                {
                    return ApiResponseHelper.Failure<RoleDto>(
                        "Modified by user not found.",
                        "MODIFIED_BY_USER_NOT_FOUND",
                        $"User with ID {updateRoleDto.ModifiedBy} does not exist.");
                }


                role.RoleName = updateRoleDto.RoleName;
                role.Description = updateRoleDto.Description;
                role.IsActive = updateRoleDto.IsActive;
                role.ModifiedBy = updateRoleDto.ModifiedBy;
                role.ModifiedAt = DateTime.UtcNow;


                await db.SaveChangesAsync();


                var updatedRole = await db.Roles
                    .FirstOrDefaultAsync(x => x.RoleId == id);

                var result = mapper.Map<RoleDto>(updatedRole);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Role updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<RoleDto>(
                    "Failed to update role.",
                    "ROLE_UPDATE_ERROR",
                    ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await db.Roles
                    .FirstOrDefaultAsync(x => x.RoleId == id);

                if (role == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Role not found.",
                        "ROLE_NOT_FOUND",
                        $"Role with ID {id} does not exist.");
                }


                db.Roles.Remove(role);

                await db.SaveChangesAsync();


                return ApiResponseHelper.SuccessRes(
                    true,
                    "Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete role.",
                    "ROLE_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}