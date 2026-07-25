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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Fincore.Infrastructure.Services.Capex
{
    public class QuotationService : IQuotationService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public QuotationService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateQuotation(QuotationDTOPost quotation)
        {
            var add = map.Map<Quotation>(quotation);
            add.CreatedAt = DateTime.Now;
            await db.Quotations.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(quotation);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(), "Quotation raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation failed", "ERROR_OCCURED", "Failed to raise Quotation");
            }
        }

        public async Task<ApiResponse<string>> DeleteQuotation(int id)
        {
            var quotation = await db.Quotations.FindAsync(id);

            if (quotation == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete Quotation", "NOT_FOUND", $"Quotation with id {id} Not found");
            }

            var rfqVendor = await db.RFQVendors
              .FirstOrDefaultAsync(x =>
              x.RFQId == quotation.RFQId &&
              x.VendorId == quotation.VendorId);

            if (rfqVendor != null && rfqVendor.ResponseStatus == "Responded")
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Already Submitted",
                    "ALREADY_SUBMITTED",
                    "Submitted quotations cannot be modified.");
            }

            db.Quotations.Remove(quotation);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"Quotation Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<QuotationDTOGet>>> GetQuotation(int page, int pagesize)
        {
            string cacheKey = $"Quotation_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<QuotationDTOGet> Quotationlist))
            {
                return ApiResponseHelper.SuccessRes(
                    Quotationlist,
                    "Quotations fetched successfully",
                    Quotationlist.Count,
                    new { page = page, pagesize = pagesize });
            }

            Quotationlist = await db.Quotations
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<QuotationDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (Quotationlist == null & !Quotationlist.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationDTOGet>>(
                    "Quotations not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, Quotationlist);

            return ApiResponseHelper.SuccessRes(
                Quotationlist,
                "Quotations fetched successfully",
                Quotationlist.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<QuotationDTOGet>> GetQuotationById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out QuotationDTOGet quotation))
            {
                return ApiResponseHelper.SuccessRes(
                    quotation,
                    $"Quotation with Id {id} found",
                    1);
            }

            quotation = await db.Quotations
                .Where(x => x.QuotationId == id)
                .ProjectTo<QuotationDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (quotation == null)
            {
                return ApiResponseHelper.Failure<QuotationDTOGet>(
                    "Quotation Record not found",
                    "NOT_FOUND",
                    $"Quotation with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                quotation,
                $"Quotation with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateQuotation(int id, QuotationDTOPost quotation)
        {
            var update = await db.Quotations.FirstOrDefaultAsync(x => x.QuotationId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Not Found",
                    "NOT_FOUND",
                    $"Quotation with id {id} Not found");
            }
            var rfqVendor = await db.RFQVendors
              .FirstOrDefaultAsync(x =>
              x.RFQId == update.RFQId &&
              x.VendorId == update.VendorId);

            if (rfqVendor != null && rfqVendor.ResponseStatus == "Responded")
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Already Submitted",
                    "ALREADY_SUBMITTED",
                    "Submitted quotations cannot be modified.");
            }

            update.ModifiedAt = DateTime.UtcNow;

            map.Map(quotation, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"Quotation Updated Successfully with id {id}");
        }

        public async Task<ApiResponse<string>> SubmitQuotation(int id)
        {
            var quotation = await db.Quotations
                .FirstOrDefaultAsync(x => x.QuotationId == id);

            if (quotation == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Not Found",
                    "NOT_FOUND",
                    $"Quotation with id {id} not found");
            }

            var rfqVendor = await db.RFQVendors
                .FirstOrDefaultAsync(x =>
                    x.RFQId == quotation.RFQId &&
                    x.VendorId == quotation.VendorId);

            if (rfqVendor == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Vendor Not Found",
                    "NOT_FOUND",
                    "Vendor invitation not found.");
            }

            if (rfqVendor.ResponseStatus == "Responded")
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Already Submitted",
                    "ALREADY_SUBMITTED",
                    "Quotation has already been submitted.");
            }

            rfqVendor.ResponseStatus = "Responded";

            await db.SaveChangesAsync();

            memoryCache.Remove($"{id}");

            return ApiResponseHelper.SuccessRes(
                $"Quotation submitted successfully.");
        }

        public async Task<ApiResponse<List<QuotationDTOGet>>> GetVendorQuotations(int vendorId)
        {
            var quotations = await db.Quotations
                .Where(x => x.VendorId == vendorId)
                .ProjectTo<QuotationDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!quotations.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationDTOGet>>(
                    "No Quotations Found",
                    "EMPTY_DATA",
                    "No quotations found for this vendor.");
            }

            return ApiResponseHelper.SuccessRes(
                quotations,
                "Vendor quotations fetched successfully",
                quotations.Count);
        }

        public async Task<ApiResponse<List<QuotationDTOGet>>> GetRFQQuotations(int rfqId)
        {
            var quotations = await db.Quotations
                .Where(x => x.RFQId == rfqId)
                .ProjectTo<QuotationDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!quotations.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationDTOGet>>(
                    "No Quotations Found",
                    "EMPTY_DATA",
                    "No quotations received for this RFQ.");
            }

            return ApiResponseHelper.SuccessRes(
                quotations,
                "RFQ quotations fetched successfully",
                quotations.Count);
        }
    }
}