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
    public class ApprovalFlowService : IApprovalFlowService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public ApprovalFlowService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateApprovalFlow(ApprovalFlowDTOPost approvalFlow)
        {
            var add = map.Map<ApprovalFlow>(approvalFlow);
            add.CreatedAt = DateTime.Now;

            await db.ApprovalFlows.AddAsync(add);
            var result = await db.SaveChangesAsync();

            memoryCache.Remove(approvalFlow);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(),
                    "Approval Flow Created Successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Creation Failed",
                    "ERROR_OCCURED",
                    "Failed to Create Approval Flow");
            }
        }

        public async Task<ApiResponse<string>> DeleteApprovalFlow(int id)
        {
            var approvalFlow = await db.ApprovalFlows.FindAsync(id);

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete Approval Flow",
                    "NOT_FOUND",
                    $"Approval Flow with id {id} Not found");
            }

            db.ApprovalFlows.Remove(approvalFlow);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes(
                $"Approval Flow Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<ApprovalFlowDTOGet>>> GetApprovalFlow(int page, int pagesize)
        {
            string cacheKey = $"ApprovalFlow_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<ApprovalFlowDTOGet> approvalFlowList))
            {
                return ApiResponseHelper.SuccessRes(
                    approvalFlowList,
                    "Approval Flow fetched successfully",
                    approvalFlowList.Count,
                    new { page = page, pagesize = pagesize });
            }

            approvalFlowList = await db.ApprovalFlows
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<ApprovalFlowDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (approvalFlowList == null & !approvalFlowList.Any())
            {
                return ApiResponseHelper.Failure<List<ApprovalFlowDTOGet>>(
                    "Approval Flow not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, approvalFlowList);

            return ApiResponseHelper.SuccessRes(
                approvalFlowList,
                "Approval Flow fetched successfully",
                approvalFlowList.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<ApprovalFlowDTOGet>> GetApprovalFlowById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out ApprovalFlowDTOGet approvalFlow))
            {
                return ApiResponseHelper.SuccessRes(
                    approvalFlow,
                    $"Approval Flow with Id {id} found",
                    1);
            }

            approvalFlow = await db.ApprovalFlows
                .Where(x => x.ApprovalFlowId == id)
                .ProjectTo<ApprovalFlowDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<ApprovalFlowDTOGet>(
                    "Approval Flow Record not found",
                    "NOT_FOUND",
                    $"Approval Flow with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                approvalFlow,
                $"Approval Flow with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateApprovalFlow(int id, ApprovalFlowDTOPost approvalFlow)
        {
            var update = await db.ApprovalFlows
                .FirstOrDefaultAsync(x => x.ApprovalFlowId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Not Found",
                    "NOT_FOUND",
                    $"Approval Flow with id {id} Not found");
            }

            update.ModifiedAt = DateTime.UtcNow;

            map.Map(approvalFlow, update);

            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes(
                $"Approval Flow Updated Successfully with id {id}");
        }

        public async Task<ApiResponse<ApprovalFlowDTOGet>> GetApprovalFlowByAmount(decimal amount)
        {
            var approvalFlow = await db.ApprovalFlows
                .Where(x => amount >= x.MinAmount && amount <= x.MaxAmount)
                .ProjectTo<ApprovalFlowDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<ApprovalFlowDTOGet>(
                    "Approval Flow Not Found",
                    "NOT_FOUND",
                    $"No Approval Flow found for amount {amount}");
            }

            return ApiResponseHelper.SuccessRes(
                approvalFlow,
                "Approval Flow fetched successfully",
                1);
        }

        public async Task<ApiResponse<List<ApprovalFlowDTOGet>>> GetApprovalFlowByRole(int roleId)
        {
            var approvalFlowList = await db.ApprovalFlows
                .Where(x => x.RequiredRoleId == roleId)
                .ProjectTo<ApprovalFlowDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (approvalFlowList == null || !approvalFlowList.Any())
            {
                return ApiResponseHelper.Failure<List<ApprovalFlowDTOGet>>(
                    "Approval Flow Not Found",
                    "NOT_FOUND",
                    $"No Approval Flow found for Role Id {roleId}");
            }

            return ApiResponseHelper.SuccessRes(
                approvalFlowList,
                "Approval Flow fetched successfully",
                approvalFlowList.Count);
        }
    }
}