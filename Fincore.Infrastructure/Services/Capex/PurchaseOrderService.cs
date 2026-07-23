using AutoMapper;
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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        private const string PurchaseOrderCacheKey = "PurchaseOrder";


        public PurchaseOrderService(AppDbContext db,IMapper mapper,IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }


        public async Task<ApiResponse<PurchaseOrderDTO>> AddPurchaseOrder(PurchaseOrderDTO dto)
        {
            var data = mapper.Map<PurchaseOrder>(dto);

            await db.PurchaseOrders.AddAsync(data);

            var result = await db.SaveChangesAsync();


            if (result > 0)
            {
                cache.Remove(PurchaseOrderCacheKey);

                return ApiResponseHelper.SuccessRes(
                    mapper.Map<PurchaseOrderDTO>(data),
                    "Purchase Order Created Successfully"
                );
            }


            return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                "Purchase Order Not Created",
                "500",
                "Error while saving data"
            );
        }



        public async Task<ApiResponse<PurchaseOrderDTO>> GetPurchaseOrder(int id)
        {
            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Retrieved Successfully"
            );
        }



        public async Task<ApiResponse<List<PurchaseOrderDTO>>> GetAllPurchaseOrder(
     int page,
     int pageSize)
        {
            string cacheKey = $"{PurchaseOrderCacheKey}_{page}_{pageSize}";


            if (cache.TryGetValue(cacheKey, out List<PurchaseOrderDTO> purchaseOrders))
            {
                var totalRecords = await db.PurchaseOrders.CountAsync();

                return ApiResponseHelper.SuccessRes(
                    purchaseOrders,
                    "Purchase Orders Retrieved Successfully",
                    totalRecords,
                    new
                    {
                        page = page,
                        pageSize = pageSize
                    }
                );
            }


            var data = await db.PurchaseOrders
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();


            purchaseOrders = mapper.Map<List<PurchaseOrderDTO>>(data);


            if (!purchaseOrders.Any())
            {
                return ApiResponseHelper.Failure<List<PurchaseOrderDTO>>(
                    "Purchase Orders Not Found",
                    "PO_NOT_FOUND",
                    "No Purchase Order records found"
                );
            }


            cache.Set(
                cacheKey,
                purchaseOrders,
                TimeSpan.FromMinutes(5)
            );


            var totalRecord = await db.PurchaseOrders.CountAsync();


            return ApiResponseHelper.SuccessRes(
                purchaseOrders,
                "Purchase Orders Retrieved Successfully",
                totalRecord,
                new
                {
                    page = page,
                    pageSize = pageSize
                }
            );
        }


        public async Task<ApiResponse<PurchaseOrderDTO>> UpdatePurchaseOrder(
            int id,
            PurchaseOrderDTO dto)
        {

            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            mapper.Map(dto, data);

            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);


            return ApiResponseHelper.SuccessRes(
                mapper.Map<PurchaseOrderDTO>(data),
                "Purchase Order Updated Successfully"
            );
        }



        public async Task<ApiResponse<PurchaseOrderDTO>> DeletePurchaseOrder(int id)
        {

            var data = await db.PurchaseOrders
                               .FirstOrDefaultAsync(x => x.POId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<PurchaseOrderDTO>(
                    "Purchase Order Not Found",
                    "404",
                    "Record not found"
                );
            }


            var result = mapper.Map<PurchaseOrderDTO>(data);


            db.PurchaseOrders.Remove(data);

            await db.SaveChangesAsync();


            cache.Remove(PurchaseOrderCacheKey);


            return ApiResponseHelper.SuccessRes(
                result,
                "Purchase Order Deleted Successfully"
            );
        }
    }
}