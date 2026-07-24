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
    public class VendorService : IVendorService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public VendorService(
            AppDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }


        // GET ALL VENDORS
        public async Task<ApiResponse<List<VendorDto>>> GetAllVendorsAsync(
            int pageNumber,
            int pageSize)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;

                if (pageSize <= 0)
                    pageSize = 10;

                var totalRecords = await db.Vendors.CountAsync();

                var vendors = await db.Vendors
                    .Include(x => x.VendorCategory)
                    .Include(x => x.Company)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .OrderBy(x => x.VendorId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var vendorDtos =
                    mapper.Map<List<VendorDto>>(vendors);

                var metadata = new
                {
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(
                        totalRecords / (double)pageSize)
                };

                return ApiResponseHelper.SuccessRes(
                    vendorDtos,
                    "Vendors retrieved successfully.",
                    totalRecords,
                    metadata);
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<List<VendorDto>>(
                    "Failed to retrieve vendors.",
                    "VENDOR_GET_ERROR",
                    ex.Message);
            }
        }


        // GET VENDOR BY ID
        public async Task<ApiResponse<VendorDto>> GetVendorByIdAsync(
            int id)
        {
            try
            {
                var vendor = await db.Vendors
                    .Include(x => x.VendorCategory)
                    .Include(x => x.Company)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .FirstOrDefaultAsync(
                        x => x.VendorId == id);

                if (vendor == null)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor not found.",
                        "VENDOR_NOT_FOUND",
                        $"Vendor with ID {id} does not exist.");
                }

                var vendorDto =
                    mapper.Map<VendorDto>(vendor);

                return ApiResponseHelper.SuccessRes(
                    vendorDto,
                    "Vendor retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorDto>(
                    "Failed to retrieve vendor.",
                    "VENDOR_GET_ERROR",
                    ex.Message);
            }
        }


        // CREATE VENDOR
        public async Task<ApiResponse<VendorDto>> CreateVendorAsync(
            CreateVendorDto createVendorDto)
        {
            try
            {
                // Check Vendor Category
                var vendorCategoryExists =
                    await db.VendorCategories
                        .AnyAsync(x =>
                            x.VendorCategoryId ==
                            createVendorDto.VendorCategoryId);

                if (!vendorCategoryExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor category not found.",
                        "VENDOR_CATEGORY_NOT_FOUND",
                        $"Vendor category with ID {createVendorDto.VendorCategoryId} does not exist.");
                }


                // Check Company
                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId ==
                        createVendorDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {createVendorDto.CompanyId} does not exist.");
                }


                // Check duplicate Vendor Code
                var vendorCodeExists = await db.Vendors
                    .AnyAsync(x =>
                        x.VendorCode ==
                        createVendorDto.VendorCode);

                if (vendorCodeExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor code already exists.",
                        "DUPLICATE_VENDOR_CODE",
                        $"Vendor with code {createVendorDto.VendorCode} already exists.");
                }


                // Check CreatedBy
                var createdByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId ==
                        createVendorDto.CreatedBy);

                if (!createdByExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Created by user not found.",
                        "CREATED_BY_USER_NOT_FOUND",
                        $"User with ID {createVendorDto.CreatedBy} does not exist.");
                }


                var vendor =
                    mapper.Map<Vendor>(createVendorDto);

                vendor.VendorId = 0;

                // Auditing
                vendor.CreatedAt = DateTime.UtcNow;
                vendor.ModifiedAt = DateTime.UtcNow;
                vendor.ModifiedBy = createVendorDto.CreatedBy;

                await db.Vendors.AddAsync(vendor);
                await db.SaveChangesAsync();


                var createdVendor = await db.Vendors
                    .Include(x => x.VendorCategory)
                    .Include(x => x.Company)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .FirstOrDefaultAsync(
                        x => x.VendorId == vendor.VendorId);

                var result =
                    mapper.Map<VendorDto>(createdVendor);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Vendor created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorDto>(
                    "Failed to create vendor.",
                    "VENDOR_CREATE_ERROR",
                    ex.Message);
            }
        }


        // UPDATE VENDOR
        public async Task<ApiResponse<VendorDto>> UpdateVendorAsync(
            int id,
            UpdateVendorDto updateVendorDto)
        {
            try
            {
                // Check Vendor
                var vendor = await db.Vendors
                    .FirstOrDefaultAsync(
                        x => x.VendorId == id);

                if (vendor == null)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor not found.",
                        "VENDOR_NOT_FOUND",
                        $"Vendor with ID {id} does not exist.");
                }


                // Check Vendor Category
                var vendorCategoryExists =
                    await db.VendorCategories
                        .AnyAsync(x =>
                            x.VendorCategoryId ==
                            updateVendorDto.VendorCategoryId);

                if (!vendorCategoryExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor category not found.",
                        "VENDOR_CATEGORY_NOT_FOUND",
                        $"Vendor category with ID {updateVendorDto.VendorCategoryId} does not exist.");
                }


                // Check Company
                var companyExists = await db.Companies
                    .AnyAsync(x =>
                        x.CompanyId ==
                        updateVendorDto.CompanyId);

                if (!companyExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Company not found.",
                        "COMPANY_NOT_FOUND",
                        $"Company with ID {updateVendorDto.CompanyId} does not exist.");
                }


                // Check duplicate Vendor Code
                var vendorCodeExists = await db.Vendors
                    .AnyAsync(x =>
                        x.VendorCode ==
                        updateVendorDto.VendorCode &&
                        x.VendorId != id);

                if (vendorCodeExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Vendor code already exists.",
                        "DUPLICATE_VENDOR_CODE",
                        $"Vendor with code {updateVendorDto.VendorCode} already exists.");
                }


                // Check ModifiedBy
                var modifiedByExists = await db.Users
                    .AnyAsync(x =>
                        x.UserId ==
                        updateVendorDto.ModifiedBy);

                if (!modifiedByExists)
                {
                    return ApiResponseHelper.Failure<VendorDto>(
                        "Modified by user not found.",
                        "MODIFIED_BY_USER_NOT_FOUND",
                        $"User with ID {updateVendorDto.ModifiedBy} does not exist.");
                }


                vendor.VendorCode =
                    updateVendorDto.VendorCode;

                vendor.VendorCategoryId =
                    updateVendorDto.VendorCategoryId;

                vendor.CompanyId =
                    updateVendorDto.CompanyId;

                vendor.BankAccount =
                    updateVendorDto.BankAccount;

                vendor.PAN =
                    updateVendorDto.PAN;

                vendor.PerformanceScore =
                    updateVendorDto.PerformanceScore;

                vendor.IsVerified =
                    updateVendorDto.IsVerified;

                vendor.IsActive =
                    updateVendorDto.IsActive;

                // Auditing
                vendor.ModifiedBy =
                    updateVendorDto.ModifiedBy;

                vendor.ModifiedAt =
                    DateTime.UtcNow;


                await db.SaveChangesAsync();


                var updatedVendor = await db.Vendors
                    .Include(x => x.VendorCategory)
                    .Include(x => x.Company)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.ModifiedByUser)
                    .FirstOrDefaultAsync(
                        x => x.VendorId == id);

                var result =
                    mapper.Map<VendorDto>(updatedVendor);

                return ApiResponseHelper.SuccessRes(
                    result,
                    "Vendor updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<VendorDto>(
                    "Failed to update vendor.",
                    "VENDOR_UPDATE_ERROR",
                    ex.Message);
            }
        }


        // DELETE VENDOR
        public async Task<ApiResponse<bool>> DeleteVendorAsync(
            int id)
        {
            try
            {
                var vendor = await db.Vendors
                    .FirstOrDefaultAsync(
                        x => x.VendorId == id);

                if (vendor == null)
                {
                    return ApiResponseHelper.Failure<bool>(
                        "Vendor not found.",
                        "VENDOR_NOT_FOUND",
                        $"Vendor with ID {id} does not exist.");
                }

                db.Vendors.Remove(vendor);

                await db.SaveChangesAsync();

                return ApiResponseHelper.SuccessRes(
                    true,
                    "Vendor deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Failed to delete vendor.",
                    "VENDOR_DELETE_ERROR",
                    ex.Message);
            }
        }
    }
}