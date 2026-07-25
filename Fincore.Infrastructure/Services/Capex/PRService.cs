using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.AutoMapper.Capex;
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
    public class PRService : IPRService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;
        public PRService(AppDbContext db, IMapper map, IMemoryCache memoryCache) 
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreatePR(PRDTOPost pr)
        {
            var add = map.Map<PurchaseRequisition>(pr);
            add.CreatedAt = DateTime.Now;
            add.ApprovalStatus = "Draft";
            await db.PurchaseRequisitions.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(pr);

            if (result > 0)
            {
              return  ApiResponseHelper.SuccessRes(
                    result.ToString(), "Purchase Requisition raised successfully");
            }
            else
            {
               return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition failed", "ERROR_OCCURED", "Failed to raise Purchase Requisition");
            }
        }

        public async Task<ApiResponse<string>> DeletePR(int id)
        {
            var pr = await db.PurchaseRequisitions.FindAsync(id);

            if (pr == null)
            {
                return ApiResponseHelper.Failure<string>(
                                   "Cannot Delete Purchase Requistion", "NOT_FOUND", $"Purchase Requistion with id {id} Not found");

            }

            db.PurchaseRequisitions.Remove(pr);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"Purchase Requistion Deleted Successfully with id {id}");

        }

        public async Task<ApiResponse<List<PRDTOGet>>> GetPR(int page, int pagesize)
        {
            string cacheKey = $"PR_{page}_{pagesize}";

            if(memoryCache.TryGetValue(cacheKey, out List<PRDTOGet> PRlist))
            {
                return ApiResponseHelper.SuccessRes(
                    PRlist,"Purchase Requistions fetched successfully", PRlist.Count, new {page=page,pagesize=pagesize} );
            }

            PRlist = await db.PurchaseRequisitions
                .Skip((page - 1 ) * pagesize).Take(pagesize)
                .ProjectTo<PRDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if(PRlist==null & !PRlist.Any())
            {
                return ApiResponseHelper.Failure<List<PRDTOGet>>(
                    "Purchase Requistions not found", "EMPTY_DATA", "No Data to show");
            }

            memoryCache.Set(cacheKey, PRlist);

            return ApiResponseHelper.SuccessRes(
                    PRlist, "Purchase Requistions fetched successfully", PRlist.Count, new { page = page, pagesize = pagesize });

        }

        public async Task<ApiResponse<PRDTOGet>> GetPRById(int id)
        {
            string cacheKey = $"{id}";

            if(memoryCache.TryGetValue(cacheKey,out PRDTOGet pr))
            {
                return ApiResponseHelper.SuccessRes(
                    pr, $"Purchase Requistion with Id {id} found",1);
            }

            pr = await db.PurchaseRequisitions
                .Where(x=> x.PurchaseRequisitionId == id)
                .ProjectTo<PRDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if(pr==null)
            {
                return ApiResponseHelper.Failure<PRDTOGet>(
                    "Purchase Requistion Record not found", "NOT_FOUND", $"Purchase Requistion with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                    pr, $"Purchase Requistion with Id {id} found", 1);
        }

        public async Task<ApiResponse<string>> UpdatePR(int id, PRDTOPost pr)
        {
           var update = await db.PurchaseRequisitions.FirstOrDefaultAsync(x=> x.PurchaseRequisitionId==id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requistion Not Found", "NOT_FOUND", $"Purchase Requistion with id {id} Not found");
            }

            update.ModifiedAt = DateTime.UtcNow;

            map.Map(pr, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"Purchase Requistion Updated Successfully with id {id}");

        }

        //Submit PR
        public async Task<ApiResponse<string>> SubmitPR(int id)
        {
            var res = await db.PurchaseRequisitions
                .FirstOrDefaultAsync(x => x.PurchaseRequisitionId == id);

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Not Found",
                    "NOT_FOUND",
                    $"Purchase Requisition with id {id} Not found");
            }

            if (res.ApprovalStatus == "Submitted")
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Already Submitted",
                    "ALREADY_SUBMITTED",
                    $"Purchase Requisition with id {id} is already submitted");
            }

            if (res.ApprovalStatus != "Submitted" || res.ApprovalStatus != "Draft" )
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Already Approved or Rejected",
                    "ALREADY_SUBMITTED",
                    $"Purchase Requisition with id {id} is already Approved or Rejected");
            }

            res.ApprovalStatus = "Submitted";
            res.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            memoryCache.Remove($"{id}");

            return ApiResponseHelper.SuccessRes(
                $"Purchase Requisition Submitted Successfully with id {id}");
        }

        //Approve Purchase Requisition
        public async Task<ApiResponse<string>> ApprovePR(int id)
        {
            var res = await db.PurchaseRequisitions.FirstOrDefaultAsync(x => x.PurchaseRequisitionId == id);

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Not Found",
                    "NOT_FOUND",
                    $"Purchase Requisition with id {id} Not found");
            }

            if (res.ApprovalStatus != "Submitted")
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Request",
                    "INVALID_STATUS",
                    "Only Submitted Purchase Requisitions can be Approved");
            }

            res.ApprovalStatus = "Approved";
            res.ApprovedAt = DateTime.Now;
            res.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes(
                $"Purchase Requisition Approved Successfully with id {id}");
        }

        //Reject Purchase Requisition
        public async Task<ApiResponse<string>> RejectPR(int id)
        {
            var res = await db.PurchaseRequisitions.FirstOrDefaultAsync(x => x.PurchaseRequisitionId == id);

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition Not Found",
                    "NOT_FOUND",
                    $"Purchase Requisition with id {id} Not found");
            }

            if (res.ApprovalStatus != "Submitted")
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Request",
                    "INVALID_STATUS",
                    "Only Submitted Purchase Requisitions can be Rejected");
            }

            res.ApprovalStatus = "Rejected";
            res.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes(
                $"Purchase Requisition Rejected Successfully with id {id}");
        }
    }
}
