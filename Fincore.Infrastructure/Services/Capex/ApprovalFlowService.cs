using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.DTO.Reports;
using Fincore.Application.Interfaces.ICapex;
using Fincore.Domain.Enums;
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
            if (approvalFlow.MinAmount > approvalFlow.MaxAmount)
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Amount Range",
                    "INVALID_RANGE",
                    $"MinAmount ({approvalFlow.MinAmount}) cannot be greater than MaxAmount ({approvalFlow.MaxAmount})");
            }

            var add = map.Map<ApprovalFlow>(approvalFlow);
            add.CreatedAt = DateTime.UtcNow;           

            await db.ApprovalFlows.AddAsync(add);
            var result = await db.SaveChangesAsync();

            memoryCache.Remove($"ApprovalFlow_{add.ApprovalFlowId}");

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

            if (approvalFlow.IsActive == 0)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow is already deleted",
                    "ALREADY_DELETED",
                    $"Approval Flow with id {id} has already been deleted");
            }

            approvalFlow.IsActive = 0;
            approvalFlow.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes(
                $"Approval Flow Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<ApprovalFlowDTOGet>>> GetApprovalFlow(int page, int pagesize, IsActive? isActive)
        {
            string cacheKey = $"ApprovalFlow_{page}_{pagesize}_{isActive}";

            if (memoryCache.TryGetValue(cacheKey, out List<ApprovalFlowDTOGet> approvalFlowList))
            {
                return ApiResponseHelper.SuccessRes(
                    approvalFlowList,
                    "Approval Flow fetched successfully",
                    approvalFlowList.Count,
                    new { page = page, pagesize = pagesize });
            }

            if (page < 1)
            {
                return ApiResponseHelper.Failure<List<ApprovalFlowDTOGet>>(
                    "Invalid page number.", "INVALID_PAGE", "Page number must be greater than or equal to 1.");
            }

            if (pagesize < 1)
            {
                return ApiResponseHelper.Failure<List<ApprovalFlowDTOGet>>(
                    "Invalid page size.", "INVALID_PAGE_SIZE", "Page size must be greater than or equal to 1.");
            }

            IQueryable<ApprovalFlow> query = db.ApprovalFlows.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == (int)isActive.Value);
            }

            approvalFlowList = await query
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
                .Where(x => x.ApprovalFlowId == id && x.IsActive==1)
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
                .FirstOrDefaultAsync(x => x.ApprovalFlowId == id && x.IsActive == 1);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Not Found",
                    "NOT_FOUND",
                    $"Approval Flow with id {id} Not found");
            }

            if (approvalFlow.MinAmount > approvalFlow.MaxAmount)
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Amount Range",
                    "INVALID_RANGE",
                    $"MinAmount ({approvalFlow.MinAmount}) cannot be greater than MaxAmount ({approvalFlow.MaxAmount})");
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
                .Where(x => amount >= x.MinAmount && amount <= x.MaxAmount && x.IsActive==1)
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
                .Where(x => x.RequiredRoleId == roleId && x.IsActive == 1)
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