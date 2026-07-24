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
    public class QuotationItemService : IQuotationItemService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public QuotationItemService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateQuotationItem(QuotationItemDTOPost quotationItem)
        {
            var add = map.Map<QuotationItem>(quotationItem);
            
            await db.QuotationItems.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(quotationItem);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(), "Quotation Item raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Item failed", "ERROR_OCCURED", "Failed to raise Quotation Item");
            }
        }

        public async Task<ApiResponse<string>> DeleteQuotationItem(int id)
        {
            var quotationItem = await db.QuotationItems.FindAsync(id);

            if (quotationItem == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete Quotation Item", "NOT_FOUND", $"Quotation Item with id {id} Not found");
            }

            db.QuotationItems.Remove(quotationItem);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"Quotation Item Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<QuotationItemDTOGet>>> GetQuotationItem(int page, int pagesize)
        {
            string cacheKey = $"QuotationItem_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<QuotationItemDTOGet> QuotationItemlist))
            {
                return ApiResponseHelper.SuccessRes(
                    QuotationItemlist,
                    "Quotation Items fetched successfully",
                    QuotationItemlist.Count,
                    new { page = page, pagesize = pagesize });
            }

            QuotationItemlist = await db.QuotationItems
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<QuotationItemDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (QuotationItemlist == null & !QuotationItemlist.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationItemDTOGet>>(
                    "Quotation Items not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, QuotationItemlist);

            return ApiResponseHelper.SuccessRes(
                QuotationItemlist,
                "Quotation Items fetched successfully",
                QuotationItemlist.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<QuotationItemDTOGet>> GetQuotationItemById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out QuotationItemDTOGet quotationItem))
            {
                return ApiResponseHelper.SuccessRes(
                    quotationItem,
                    $"Quotation Item with Id {id} found",
                    1);
            }

            quotationItem = await db.QuotationItems
                .Where(x => x.QuotationItemId == id)
                .ProjectTo<QuotationItemDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (quotationItem == null)
            {
                return ApiResponseHelper.Failure<QuotationItemDTOGet>(
                    "Quotation Item Record not found",
                    "NOT_FOUND",
                    $"Quotation Item with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                quotationItem,
                $"Quotation Item with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateQuotationItem(int id, QuotationItemDTOPost quotationItem)
        {
            var update = await db.QuotationItems.FirstOrDefaultAsync(x => x.QuotationItemId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Item Not Found",
                    "NOT_FOUND",
                    $"Quotation Item with id {id} Not found");
            }

            map.Map(quotationItem, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"Quotation Item Updated Successfully with id {id}");
        }
    }
}