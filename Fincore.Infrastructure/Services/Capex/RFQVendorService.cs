using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services.Capex
{
    public class RFQVendorService : IRFQVendorService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public RFQVendorService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateRFQVendor(RFQVendorDTOPost rfqVendor)
        {
            var add = map.Map<RFQVendor>(rfqVendor);
            await db.RFQVendors.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(rfqVendor);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(), "RFQ Vendor raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Vendor failed", "ERROR_OCCURED", "Failed to raise RFQ Vendor");
            }
        }

        public async Task<ApiResponse<string>> DeleteRFQVendor(int id)
        {
            var rfqVendor = await db.RFQVendors.FindAsync(id);

            if (rfqVendor == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete RFQ Vendor", "NOT_FOUND", $"RFQ Vendor with id {id} Not found");
            }

            db.RFQVendors.Remove(rfqVendor);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"RFQ Vendor Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<RFQVendorDTOGet>>> GetRFQVendor(int page, int pagesize)
        {
            string cacheKey = $"RFQVendor_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<RFQVendorDTOGet> RFQVendorlist))
            {
                return ApiResponseHelper.SuccessRes(
                    RFQVendorlist,
                    "RFQ Vendors fetched successfully",
                    RFQVendorlist.Count,
                    new { page = page, pagesize = pagesize });
            }

            RFQVendorlist = await db.RFQVendors
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<RFQVendorDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (RFQVendorlist == null & !RFQVendorlist.Any())
            {
                return ApiResponseHelper.Failure<List<RFQVendorDTOGet>>(
                    "RFQ Vendors not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, RFQVendorlist);

            return ApiResponseHelper.SuccessRes(
                RFQVendorlist,
                "RFQ Vendors fetched successfully",
                RFQVendorlist.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<RFQVendorDTOGet>> GetRFQVendorById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out RFQVendorDTOGet rfqVendor))
            {
                return ApiResponseHelper.SuccessRes(
                    rfqVendor,
                    $"RFQ Vendor with Id {id} found",
                    1);
            }

            rfqVendor = await db.RFQVendors
                .Where(x => x.RFQVendorId == id)
                .ProjectTo<RFQVendorDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (rfqVendor == null)
            {
                return ApiResponseHelper.Failure<RFQVendorDTOGet>(
                    "RFQ Vendor Record not found",
                    "NOT_FOUND",
                    $"RFQ Vendor with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                rfqVendor,
                $"RFQ Vendor with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateRFQVendor(int id, RFQVendorDTOPost rfqVendor)
        {
            var update = await db.RFQVendors.FirstOrDefaultAsync(x => x.RFQVendorId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Vendor Not Found",
                    "NOT_FOUND",
                    $"RFQ Vendor with id {id} Not found");
            }

            map.Map(rfqVendor, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"RFQ Vendor Updated Successfully with id {id}");
        }

        public async Task<ApiResponse<List<RFQVendorDTOGet>>> GetSubmittedRFQForVendor(int vendorId)
        {
            var rfqs = await db.RFQVendors
                .Where(x => x.VendorId == vendorId &&
                            x.ResponseStatus == "Responded")
                .ProjectTo<RFQVendorDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!rfqs.Any())
            {
                return ApiResponseHelper.Failure<List<RFQVendorDTOGet>>(
                    "No Submitted RFQs",
                    "EMPTY_DATA",
                    "No submitted RFQs found for this vendor.");
            }

            return ApiResponseHelper.SuccessRes(
                rfqs,
                "Submitted RFQs fetched successfully",
                rfqs.Count);
        }

        public async Task<ApiResponse<List<RFQVendorDTOGet>>> GetPendingRFQForVendor(int vendorId)
        {
            var rfqs = await db.RFQVendors
                .Where(x => x.VendorId == vendorId &&
                            x.ResponseStatus == "Invited")
                .ProjectTo<RFQVendorDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!rfqs.Any())
            {
                return ApiResponseHelper.Failure<List<RFQVendorDTOGet>>(
                    "No Pending RFQs",
                    "EMPTY_DATA",
                    "There are no pending RFQs for this vendor.");
            }

            return ApiResponseHelper.SuccessRes(
                rfqs,
                "Pending RFQs fetched successfully",
                rfqs.Count);
        }
    }
}