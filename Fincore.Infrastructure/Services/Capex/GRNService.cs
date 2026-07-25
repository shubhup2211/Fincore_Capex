using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.Services.Capex
{
    public class GRNService : IGRNService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        private const string GRNCacheKey = "GRN";
        public GRNService(AppDbContext db, IMapper mapper, IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }
        public async Task<ApiResponse<GRNDTO>> CreateGRN(GRNDTO dto)
        {

            // Duplicate GRN Code Check
            var exists = await db.GRNs
                .AnyAsync(x => x.GRNCode == dto.GRNCode);


            if (exists)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Code Already Exists",
                    "400",
                    "Duplicate GRN Code"
                );
            }



            // PO Validation

            var poExists = await db.PurchaseOrders
                .AnyAsync(x => x.POId == dto.POId);


            if (!poExists)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Invalid Purchase Order",
                    "404",
                    "Purchase Order not found"
                );
            }



            // Vendor Validation

            var vendorExists = await db.Vendors
                .AnyAsync(x => x.VendorId == dto.VendorId);


            if (!vendorExists)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Invalid Vendor",
                    "404",
                    "Vendor not found"
                );
            }



            // Default Values

            dto.GRNStatus = "OPEN";
            dto.QualityCheckStatus = "PENDING";
            dto.CreatedAt = DateTime.Now;



            var data = mapper.Map<GRN>(dto);


            await db.GRNs.AddAsync(data);


            var result = await db.SaveChangesAsync();


            if (result > 0)
            {
                cache.Remove(GRNCacheKey);


                return ApiResponseHelper.SuccessRes(
                    mapper.Map<GRNDTO>(data),
                    "GRN Created Successfully"
                );
            }



            return ApiResponseHelper.Failure<GRNDTO>(
                "GRN Creation Failed",
                "500",
                "Database error"
            );

        }

        public async Task<ApiResponse<GRNDTO>> DeleteGRN(int id)
        {
            var grn = await db.GRNs
                .FirstOrDefaultAsync(x => x.GRNId == id);


            if (grn == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Not Found",
                    "404",
                    "Record not found"
                );
            }


            // Check Invoice Exists

            var invoiceExists = await db.APInvoices
                .AnyAsync(x => x.GRNId == id);


            if (invoiceExists)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Cannot Delete GRN",
                    "400",
                    "Invoice already generated"
                );
            }



            // Soft Delete

            grn.IsActive = 0;
            grn.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            cache.Remove(GRNCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "GRN Deleted Successfully"
            );
        }

        public async Task<ApiResponse<List<GRNDTO>>> GetAllGRN(
                        int page,
                        int pageSize)
        {

            // Pagination validation

            if (page <= 0)
                page = 1;


            if (pageSize <= 0)
                pageSize = 10;



            string cacheKey = $"{GRNCacheKey}_{page}_{pageSize}";



            if (cache.TryGetValue(cacheKey, out List<GRNDTO> cachedGRN))
            {

                var totalRecords = await db.GRNs
                    .Where(x => x.IsActive != 0)
                    .CountAsync();



                return ApiResponseHelper.SuccessRes(
                    cachedGRN,
                    "GRN Retrieved Successfully",
                    totalRecords,
                    new
                    {
                        page,
                        pageSize
                    }
                );

            }




            var query = db.GRNs
                .Where(x => x.IsActive != 0);



            var totalRecord = await query.CountAsync();



            var data = await db.GRNs
                .Where(x => x.IsActive != 0)
                .OrderByDescending(x => x.GRNId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();




            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<GRNDTO>>(
                    "GRN Not Found",
                    "GRN_NOT_FOUND",
                    "No GRN records found"
                );
            }




            var grn = mapper.Map<List<GRNDTO>>(data);



            cache.Set(
                cacheKey,
                grn,
                TimeSpan.FromMinutes(5)
            );




            return ApiResponseHelper.SuccessRes(
                grn,
                "GRN Retrieved Successfully",
                totalRecord,
                new
                {
                    page,
                    pageSize
                }
            );

        }

        public async Task<ApiResponse<GRNDTO>> GetGRNById(int id)
        {
            var data = await db.GRNs.FirstOrDefaultAsync(x => x.GRNId == id);

            if (data == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                   "GRN Not Found",
                   "404",
                   "Record not found"
                   );
            }

            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(data),"GRN Retrive Successfully"
                );

        }

        public async Task<ApiResponse<GRNDTO>> UpdateGRN(GRNDTO dto, int id)
        {

            var grn = await db.GRNs
                .FirstOrDefaultAsync(x => x.GRNId == id);



            if (grn == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Not Found",
                    "404",
                    "Record not found"
                );
            }



            if (grn.GRNStatus == "CLOSED")
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Cannot Update GRN",
                    "400",
                    "Closed GRN cannot be updated"
                );
            }



            grn.ReceivedDate = dto.ReceivedDate;
            grn.ReceivedBy = dto.ReceivedBy;
            grn.Remarks = dto.Remarks;
            grn.ModifiedAt = DateTime.Now;



            await db.SaveChangesAsync();


            cache.Remove(GRNCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "GRN Updated Successfully"
            );
        }

        public async Task<ApiResponse<GRNDTO>> ApproveQualityCheck(int id)
        {

            var grn = await db.GRNs
                .FirstOrDefaultAsync(x => x.GRNId == id);



            if (grn == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Not Found",
                    "404",
                    "Invalid GRN"
                );
            }



            if (grn.QualityCheckStatus == "PASSED")
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Already Approved",
                    "400",
                    "Quality check already completed"
                );
            }



            grn.QualityCheckStatus = "PASSED";

            grn.GRNStatus = "APPROVED";

            grn.ModifiedAt = DateTime.Now;



            await db.SaveChangesAsync();


            cache.Remove(GRNCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "Quality Check Approved"
            );

        }

        public async Task<ApiResponse<GRNDTO>> RejectQualityCheck(int id)
        {

            var grn = await db.GRNs
                .FirstOrDefaultAsync(x => x.GRNId == id);



            if (grn == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Not Found",
                    "404",
                    "Invalid GRN"
                );
            }



            grn.QualityCheckStatus = "FAILED";


            grn.GRNStatus = "REJECTED";


            grn.ModifiedAt = DateTime.Now;



            await db.SaveChangesAsync();


            cache.Remove(GRNCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "Quality Check Rejected"
            );

        }

        public async Task<ApiResponse<GRNDTO>> CloseGRN(int id)
        {

            var grn = await db.GRNs
                .FirstOrDefaultAsync(x => x.GRNId == id);



            if (grn == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "GRN Not Found",
                    "404",
                    "Invalid GRN"
                );
            }



            if (grn.GRNStatus != "APPROVED")
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Cannot Close GRN",
                    "400",
                    "Only approved GRN can be closed"
                );
            }



            grn.GRNStatus = "CLOSED";

            grn.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            cache.Remove(GRNCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "GRN Closed Successfully"
            );

        }

        public async Task<ApiResponse<List<GRNDTO>>> GetGRNByStatus(string status)
        {

            var data = await db.GRNs
                .Where(x => x.GRNStatus == status)
                .ToListAsync();


            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<GRNDTO>>(data),
                "GRN Retrieved"
            );

        }

        public async Task<ApiResponse<List<GRNDTO>>> GetGRNByVendor(int vendorId)
        {

            var data = await db.GRNs
                .Where(x => x.VendorId == vendorId)
                .ToListAsync();


            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<GRNDTO>>(data),
                "Vendor GRN Retrieved"
            );

        }

        public async Task<ApiResponse<List<GRNDTO>>> GetGRNByPurchaseOrder(int poId)
        {

            var data = await db.GRNs
                .Where(x => x.POId == poId)
                .ToListAsync();



            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<GRNDTO>>(
                    "GRN Not Found",
                    "404",
                    "No GRN against this PO"
                );
            }



            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<GRNDTO>>(data),
                "GRN Retrieved Successfully"
            );

        }

        public async Task<ApiResponse<GRNDTO>> ReceiveGoods(GRNDTO dto)
        {

            // Duplicate GRN Check

            var exists = await db.GRNs
                .AnyAsync(x => x.GRNCode == dto.GRNCode);


            if (exists)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "Duplicate GRN",
                    "400",
                    "GRN Code already exists"
                );
            }



            // PO Validation

            var po = await db.PurchaseOrders
                .FirstOrDefaultAsync(x => x.POId == dto.POId);


            if (po == null)
            {
                return ApiResponseHelper.Failure<GRNDTO>(
                    "PO Not Found",
                    "404",
                    "Invalid Purchase Order"
                );
            }



            // Vendor Match Check

            //if (po.VendorId != dto.VendorId)
            //{
            //    return ApiResponseHelper.Failure<GRNDTO>(
            //        "Vendor Mismatch",
            //        "400",
            //        "Vendor does not match with PO"
            //    );
            //}



            var grn = mapper.Map<GRN>(dto);


            grn.GRNStatus = "RECEIVED";

            grn.QualityCheckStatus = "PENDING";

            grn.ReceivedDate = DateTime.Now;


            await db.GRNs.AddAsync(grn);


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
                mapper.Map<GRNDTO>(grn),
                "Goods Received Successfully"
            );

        }

        public async Task<ApiResponse<List<GRNDTO>>> GetGRNHistory(int id)
        {

            var data = await db.GRNs
                .Where(x => x.GRNId == id)
                .ToListAsync();


            if (!data.Any())
            {
                return ApiResponseHelper.Failure<List<GRNDTO>>(
                    "History Not Found",
                    "404",
                    "No history available"
                );
            }



            return ApiResponseHelper.SuccessRes(
                mapper.Map<List<GRNDTO>>(data),
                "GRN History Retrieved"
            );

        }
    }
}
