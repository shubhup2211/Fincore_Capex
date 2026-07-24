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
    public class VendorCategoryService : IVendorCategoryService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public VendorCategoryService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        // GET ALL VENDOR CATEGORIES
        public async Task<ApiResponse<List<VendorCategoryDto>>> GetAllVendorCategoriesAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords =
                    await db.VendorCategories.CountAsync();

                var vendorCategories = await db.VendorCategories
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .OrderBy(x => x.VendorCategoryId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var vendorCategoryDtos =
                    mapper.Map<List<VendorCategoryDto>>(vendorCategories);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    vendorCategoryDtos,
                    "Vendor categories retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<VendorCategoryDto>>(
                    "Failed to retrieve vendor categories.",
                    "VENDOR_CATEGORY_GET_ERROR",
                    ex.Message);
            }
        }


        // GET VENDOR CATEGORY BY ID
        public async Task<ApiResponse<VendorCategoryDto>> GetVendorCategoryByIdAsync(
            int id)
        {
            try
            {
                var vendorCategory = await db.VendorCategories
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .FirstOrDefaultAsync(
                        x => x.VendorCategoryId == id);

                if (vendorCategory == null)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Vendor category not found.",
                        "VENDOR_CATEGORY_NOT_FOUND",
                        $"Vendor category with ID {id} does not exist.");
                }

                var vendorCategoryDto =
                    mapper.Map<VendorCategoryDto>(vendorCategory);

                return ApiResponseHelper.SuccessRes(
                    vendorCategoryDto,
                    "Vendor category retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorCategoryDto>(
                    "Failed to retrieve vendor category.",
                    "VENDOR_CATEGORY_GET_ERROR",
                    ex.Message);
            }
        }


        // CREATE VENDOR CATEGORY
        public async Task<ApiResponse<VendorCategoryDto>> CreateVendorCategoryAsync(
            CreateVendorCategoryDto createVendorCategoryDto)
        {
            try
            {
                // Check duplicate Category Name
                var categoryNameExists =
                    await db.VendorCategories
                        .AnyAsync(x =>
                            x.CategoryName ==
                            createVendorCategoryDto.CategoryName);

                if (categoryNameExists)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Vendor category name already exists.",
                        "DUPLICATE_VENDOR_CATEGORY_NAME",
                        $"Vendor category with name {createVendorCategoryDto.CategoryName} already exists.");
                }


                // Check CreatedBy
                var createdByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId ==
                        createVendorCategoryDto.CreatedBy);

                if (!createdByExists)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Created by user not found.",
                        "CREATED_BY_USER_NOT_FOUND",
                        $"User with ID {createVendorCategoryDto.CreatedBy} does not exist.");
                }


                var vendorCategory =
                    mapper.Map<VendorCategory>(createVendorCategoryDto);

                vendorCategory.VendorCategoryId = 0;

                // Auditing
                vendorCategory.CreatedAt = DateTime.UtcNow;
                vendorCategory.ModifiedAt = DateTime.UtcNow;
                vendorCategory.ModifiedBy =
                    createVendorCategoryDto.CreatedBy;


                await db.VendorCategories.AddAsync(vendorCategory);
                await db.SaveChangesAsync();


                var createdVendorCategory =
                    await db.VendorCategories
                        .Include(x => x.CreatedByUser)
                        .Include(x => x.ModifiedByUser)
                        .FirstOrDefaultAsync(
                            x => x.VendorCategoryId ==
                            vendorCategory.VendorCategoryId);

                var result =
                    mapper.Map<VendorCategoryDto>(
                        createdVendorCategory);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Vendor category created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorCategoryDto>(
                    "Failed to create vendor category.",
                    "VENDOR_CATEGORY_CREATE_ERROR",
                    ex.Message);
            }
        }


        // UPDATE VENDOR CATEGORY
        public async Task<ApiResponse<VendorCategoryDto>> UpdateVendorCategoryAsync(
            int id,
            UpdateVendorCategoryDto updateVendorCategoryDto)
        {
            try
            {
                // Check Vendor Category
                var vendorCategory =
                    await db.VendorCategories
                        .FirstOrDefaultAsync(
                            x => x.VendorCategoryId == id);

                if (vendorCategory == null)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Vendor category not found.",
                        "VENDOR_CATEGORY_NOT_FOUND",
                        $"Vendor category with ID {id} does not exist.");
                }


                // Check duplicate Category Name
                var categoryNameExists =
                    await db.VendorCategories
                        .AnyAsync(x =>
                            x.CategoryName ==
                            updateVendorCategoryDto.CategoryName &&
                            x.VendorCategoryId != id);

                if (categoryNameExists)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Vendor category name already exists.",
                        "DUPLICATE_VENDOR_CATEGORY_NAME",
                        $"Vendor category with name {updateVendorCategoryDto.CategoryName} already exists.");
                }


                // Check ModifiedBy
                var modifiedByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId ==
                        updateVendorCategoryDto.ModifiedBy);

                if (!modifiedByExists)
                {
                    return ApiResponseHelper.Failure<VendorCategoryDto>(
                        "Modified by user not found.",
                        "MODIFIED_BY_USER_NOT_FOUND",
                        $"User with ID {updateVendorCategoryDto.ModifiedBy} does not exist.");
                }


                vendorCategory.CategoryName =
                    updateVendorCategoryDto.CategoryName;

                vendorCategory.Description =
                    updateVendorCategoryDto.Description;

                vendorCategory.IsActive =
                    updateVendorCategoryDto.IsActive;


                // Auditing
                vendorCategory.ModifiedBy =
                    updateVendorCategoryDto.ModifiedBy;

                vendorCategory.ModifiedAt =
                    DateTime.UtcNow;


                await db.SaveChangesAsync();


                var updatedVendorCategory =
                    await db.VendorCategories
                        .Include(x => x.CreatedByUser)
                        .Include(x => x.ModifiedByUser)
                        .FirstOrDefaultAsync(
                            x => x.VendorCategoryId == id);

                var result =
                    mapper.Map<VendorCategoryDto>(
                        updatedVendorCategory);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Vendor category updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorCategoryDto>(
                    "Failed to update vendor category.",
                    "VENDOR_CATEGORY_UPDATE_ERROR",
                    ex.Message);
            }
        }


        // DELETE VENDOR CATEGORY
        public async Task<ApiResponse<bool>> DeleteVendorCategoryAsync(
            int id)
        {
            try
            {
                var vendorCategory =
                    await db.VendorCategories
                        .FirstOrDefaultAsync(
                            x => x.VendorCategoryId == id);

                if (vendorCategory == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Vendor category not found.",
                        "VENDOR_CATEGORY_NOT_FOUND",
                        $"Vendor category with ID {id} does not exist.");
                }


                db.VendorCategories.Remove(vendorCategory);

                await db.SaveChangesAsync();


                return ApiResponseHelper.SuccessRes(
                    true,
                    "Vendor category deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete vendor category.",
                    "VENDOR_CATEGORY_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}