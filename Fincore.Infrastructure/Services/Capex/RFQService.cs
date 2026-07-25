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
    public class RFQService : IRFQService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public RFQService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateRFQ(RFQDTOPost rfq)
        {
            var add = map.Map<RFQ>(rfq);
            add.CreatedAt = DateTime.Now;
            await db.RFQs.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(rfq);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(), "RFQ raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ failed", "ERROR_OCCURED", "Failed to raise RFQ");
            }
        }

        public async Task<ApiResponse<string>> DeleteRFQ(int id)
        {
            var rfq = await db.RFQs.FindAsync(id);

            if (rfq == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete RFQ", "NOT_FOUND", $"RFQ with id {id} Not found");
            }

            db.RFQs.Remove(rfq);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"RFQ Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<RFQDTOGet>>> GetRFQ(int page, int pagesize)
        {
            string cacheKey = $"RFQ_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<RFQDTOGet> RFQlist))
            {
                return ApiResponseHelper.SuccessRes(
                    RFQlist,
                    "RFQs fetched successfully",
                    RFQlist.Count,
                    new { page = page, pagesize = pagesize });
            }

            RFQlist = await db.RFQs
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<RFQDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (RFQlist == null & !RFQlist.Any())
            {
                return ApiResponseHelper.Failure<List<RFQDTOGet>>(
                    "RFQs not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, RFQlist);

            return ApiResponseHelper.SuccessRes(
                RFQlist,
                "RFQs fetched successfully",
                RFQlist.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<RFQDTOGet>> GetRFQById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out RFQDTOGet rfq))
            {
                return ApiResponseHelper.SuccessRes(
                    rfq,
                    $"RFQ with Id {id} found",
                    1);
            }

            rfq = await db.RFQs
                .Where(x => x.RFQId == id)
                .ProjectTo<RFQDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (rfq == null)
            {
                return ApiResponseHelper.Failure<RFQDTOGet>(
                    "RFQ Record not found",
                    "NOT_FOUND",
                    $"RFQ with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                rfq,
                $"RFQ with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateRFQ(int id, RFQDTOPost rfq)
        {
            var update = await db.RFQs.FirstOrDefaultAsync(x => x.RFQId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Not Found",
                    "NOT_FOUND",
                    $"RFQ with id {id} Not found");
            }

            update.ModifiedAt = DateTime.UtcNow;

            map.Map(rfq, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"RFQ Updated Successfully with id {id}");
        }

        //Send RFQ to Vendors
        public async Task<ApiResponse<string>> SendRFQ(int id)
        {
            var rfq = await db.RFQs
                .FirstOrDefaultAsync(x => x.RFQId == id);

            if (rfq == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Not Found",
                    "NOT_FOUND",
                    $"RFQ with id {id} not found");
            }

            //Check RFQ already sent
            bool alreadySent = await db.RFQVendors
                .AnyAsync(x => x.RFQId == id);

            if (alreadySent)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Already Sent",
                    "ALREADY_SENT",
                    $"RFQ with id {id} has already been sent to vendors");
            }

            //Get PR Items
            var prItems = await db.PurchaseRequisitionItems
                .Where(x => x.PurchaseRequisitionId == rfq.PurchaseRequisitionId)
                .ToListAsync();

            if (!prItems.Any())
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Items Not Found",
                    "NOT_FOUND",
                    "No Purchase Requisition Items found for this RFQ");
            }

            //Get Category Ids
            var categoryIds = prItems
                .Select(x => x.CategoryId)
                .Distinct()
                .ToList();

            //Find Vendors matching Categories
            var vendors = await db.Vendors
                .Where(x => categoryIds.Contains(x.VendorCategoryId) && x.IsActive == 1)
                .ToListAsync();

            if (!vendors.Any())
            {
                return ApiResponseHelper.Failure<string>(
                    "Vendor Not Found",
                    "NOT_FOUND",
                    "No Vendor available for selected categories");
            }

            foreach (var vendor in vendors)
            {
                var rfqVendor = new RFQVendor
                {
                    RFQId = rfq.RFQId,
                    VendorId = vendor.VendorId,
                    InvitedAt = DateTime.Now,
                    ResponseStatus = "Pending"
                };

                await db.RFQVendors.AddAsync(rfqVendor);
            }

            await db.SaveChangesAsync();

            memoryCache.Remove($"{id}");

            return ApiResponseHelper.SuccessRes(
                $"RFQ Sent Successfully to {vendors.Count} Vendor(s)");
        }

        public async Task<ApiResponse<List<QuotationDTOGet>>> GetRFQQuotations(int id)
        {
            var rfq = await db.RFQs.FindAsync(id);

            if (rfq == null)
            {
                return ApiResponseHelper.Failure<List<QuotationDTOGet>>(
                    "RFQ Not Found",
                    "NOT_FOUND",
                    $"RFQ with id {id} not found");
            }

            var quotations = await db.Quotations
                .Where(x => x.RFQId == id)
                .ProjectTo<QuotationDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!quotations.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationDTOGet>>(
                    "Quotation Not Found",
                    "NOT_FOUND",
                    "No Quotations found for this RFQ");
            }

            return ApiResponseHelper.SuccessRes(
                quotations,
                "Quotations fetched successfully",
                quotations.Count);
        }
    }
}