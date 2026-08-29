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
    public class CapexReq : ICapexReq
    {
        private readonly AppDbContext db;
        private readonly IMapper map;
        private readonly IMemoryCache memoryCache;

        public CapexReq(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        //Raise Capex Request
        public async Task<ApiResponse<string>> RaiseCapex(CapexReqDTOPost capex)
        {
            var raise = map.Map<CapexRequest>(capex);

            raise.ApprovalStatus = "Draft";
            raise.CreatedAt = DateTime.Now;
            await db.CapexRequests.AddAsync(raise);
            int result = await db.SaveChangesAsync();
            memoryCache.Remove($"Capex_{raise.CapexRequestId}");

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    "CapexRequest Raised Successfully", "CAPEX_RAISE_DONE");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "CapexRequest Raised Failed", "ERROR_OCCURED_TRY_AGAIN", "Raise Again!");
            }
        }

        //Get Capex Request
        public async Task<ApiResponse<List<CapexReqDTOGet>>> GetCapex(int page, int pageSize)
        {
            //cache key
            string cacheKey = $"Capex_{page}_{pageSize}";

            //check cache
            if(memoryCache.TryGetValue(cacheKey, out List<CapexReqDTOGet> capexlist))
            {
                return ApiResponseHelper.SuccessRes(
                    capexlist, "Capex list Fetched Successfully", capexlist.Count, new { page = page,pageSize = pageSize });
            }

            if (page < 1)
            {
                return ApiResponseHelper.Failure<List<CapexReqDTOGet>>(
                    "Invalid page number.", "INVALID_PAGE", "Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return ApiResponseHelper.Failure<List<CapexReqDTOGet>>(
                    "Invalid page size.", "INVALID_PAGE_SIZE", "Page size must be greater than or equal to 1.");
            }

            //Use AutoMapper to fetch
            capexlist = await db.CapexRequests
                .AsNoTracking()
                .OrderBy(x => x.CapexRequestId)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ProjectTo<CapexReqDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (!capexlist.Any())
            {
                return ApiResponseHelper.Failure<List<CapexReqDTOGet>>(
                    "No Data Found for Capex", "EMPTY_DATA", "NO DATA TO SHOW");
            }

            //save data to cace
            memoryCache.Set(cacheKey, capexlist);

            return ApiResponseHelper.SuccessRes(capexlist, "Capex list Fetched Successfully", capexlist.Count, new { page = page, pageSize = pageSize });


        }

        //Get Capex Request By Id
        public async Task<ApiResponse<CapexReqDTOGet>> GetCapexById(int cid)
        {
            string cacheKey = $"{cid}";
             
            if(memoryCache.TryGetValue(cacheKey, out CapexReqDTOGet res))
            {
                return ApiResponseHelper.SuccessRes(res, "Capex Request Fetched Successfully");
            }

             res = await db.CapexRequests
                .Where(x => x.CapexRequestId == cid)
                .ProjectTo<CapexReqDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            
            if (res == null)
            {
                return ApiResponseHelper.Failure<CapexReqDTOGet>(
                    "Capex Request Not Found","NOT_FOUND", $"CapexRequest with id {cid} is not available"
                    );
            }

            memoryCache.Set(cacheKey, res);

            return ApiResponseHelper.SuccessRes(res, "Capex Request Fetched Successfully");
        }

        //Get Capex Request By Id
        public async Task<ApiResponse<string>> UpdateCapex(int id, CapexReqDTOPost capex)
        {
            var res = await db.CapexRequests.FirstOrDefaultAsync(x=> x.CapexRequestId==id);

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "CapexRequest Not Found", "NOT_FOUND", $"Capex Request with id {id} Not found");
            }

            map.Map(capex, res);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"Capex Request Updated Successfully with id {id}");

        }

        //Delete Capex Request
        public async Task<ApiResponse<string>> DeleteCapex(int id)
        {
            var res = await db.CapexRequests.FindAsync(id);

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "CapexRequest Not Found","NOT_FOUND",$"CapexRequest with id {id} not found");
            }

            db.CapexRequests.Remove(res);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"CapexRequest with id {id} Deleted successfully");

        }

        //Submit Capex Request
        public async Task<ApiResponse<string>> SubmitCapex(CapexReqDTOPost capex, int userId)
        {
            var employee = await db.Employees
             .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return ApiResponseHelper.Failure<string>("Employee not found.","NOT_FOUND","Employee is not exist");
            }

            var departmentId = employee.DepartmentId;
            // ---- Basic amount validation ----
            if (capex.Amount <= 0)
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Amount",
                    "INVALID_AMOUNT",
                    "CAPEX Amount must be greater than zero.");
            }

            // ---- Budget Line validation ----
            var budgetLine = await db.BudgetLines
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == capex.BudgetLineId &&
                    x.IsActive == 1);

            if (budgetLine == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Budget Line Not Found",
                    "BUDGET_NOT_FOUND",
                    "Budget Line is inactive or does not exist.");
            }

            decimal utilizedAmount = budgetLine.UtilizedAmount ?? 0;
            decimal remainingBudget = budgetLine.AllocatedAmount - utilizedAmount;

            if (capex.Amount > remainingBudget)
            {
                return ApiResponseHelper.Failure<string>(
                    "Budget Exceeded",
                    "BUDGET_EXCEEDED",
                    $"Available Budget is {remainingBudget}, but requested amount is {capex.Amount}.");
            }

            // ---- Approval Flow validation (must exist for the amount) ----
            var approvalFlow = await db.ApprovalFlows
                .FirstOrDefaultAsync(x =>
                    x.IsActive == 1 &&
                    capex.Amount >= x.MinAmount &&
                    capex.Amount <= x.MaxAmount);

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Not Configured",
                    "APPROVAL_FLOW_NOT_FOUND",
                    $"No Approval Flow configured for amount {capex.Amount}.");
            }

            // ---- Map DTO -> Entity ----
            var raise = map.Map<CapexRequest>(capex);

            // JWT-driven fields (never trust client)
            raise.RequestedBy = userId;
            raise.ApprovalStatus = "Submitted";
            raise.CreatedAt = DateTime.Now;
            raise.ModifiedAt = DateTime.Now;
            raise.ApprovedBy = null;
            raise.ApprovedAt = null;

            await db.CapexRequests.AddAsync(raise);
            int result = await db.SaveChangesAsync();

            memoryCache.Remove($"Capex_{raise.CapexRequestId}");

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes<string>(
                    $"CAPEX Request Submitted Successfully with id {raise.CapexRequestId}",
                    "CAPEX_SUBMIT_DONE");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "CAPEX Submit Failed",
                    "ERROR_OCCURED_TRY_AGAIN",
                    "Submit Again!");
            }
        }
        //Approve Capex Request
        public async Task<ApiResponse<string>> ApproveCapex(int id, int userId)
        {
            var res = await db.CapexRequests.FirstOrDefaultAsync(x => x.CapexRequestId == id);

            var user = await db.Users
    .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return ApiResponseHelper.Failure<string>("User not found","NOT_FOUND","User not exist");
            }

            var roleId = user.RoleId;

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "CapexRequest Not Found",
                    "NOT_FOUND",
                    $"Capex Request with id {id} Not found");
            }

            if (res.ApprovalStatus != "Submitted")
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Request",
                    "INVALID_STATUS",
                    "Only Submitted Capex Requests can be Approved");
            }

            // Find the matching Approval Flow for this amount
            var approvalFlow = await db.ApprovalFlows
                .FirstOrDefaultAsync(x =>
                    x.IsActive == 1 &&
                    res.Amount >= x.MinAmount &&
                    res.Amount <= x.MaxAmount);

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Not Configured",
                    "APPROVAL_FLOW_NOT_FOUND",
                    $"No Approval Flow configured for amount {res.Amount}.");
            }

            // Only the role assigned in the approval flow can approve
            if (approvalFlow.RequiredRoleId != roleId)
            {
                return ApiResponseHelper.Failure<string>(
                    "Unauthorized",
                    "ROLE_NOT_ALLOWED",
                    "Your role is not authorized to approve this CAPEX request.");
            }

            // Budget update
            var budgetLine = await db.BudgetLines
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == res.BudgetLineId &&
                    x.IsActive == 1);

            if (budgetLine == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Budget Line Not Found",
                    "BUDGET_NOT_FOUND",
                    "Budget Line is inactive or does not exist.");
            }

            decimal utilized = budgetLine.UtilizedAmount ?? 0;
            decimal remaining = budgetLine.AllocatedAmount - utilized;
            if (res.Amount > remaining)
            {
                return ApiResponseHelper.Failure<string>(
                    "Budget Exceeded",
                    "BUDGET_EXCEEDED",
                    $"Available Budget is {remaining}, but requested amount is {res.Amount}.");
            }

            budgetLine.UtilizedAmount = utilized + res.Amount;

            res.ApprovalStatus = "Approved";
            res.ApprovedBy = userId;
            res.ApprovedAt = DateTime.Now;
            res.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            memoryCache.Remove($"{id}");

            return ApiResponseHelper.SuccessRes<string>(
                $"Capex Request Approved Successfully with id {id}");
        }
        //Reject Capex Request
        public async Task<ApiResponse<string>> RejectCapex(int id, int userId)
        {
            var res = await db.CapexRequests.FirstOrDefaultAsync(x => x.CapexRequestId == id);


            var user = await db.Users
    .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return ApiResponseHelper.Failure<string>("User not found", "NOT_FOUND", "User not exist");
            }

            var roleId = user.RoleId;

            if (res == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "CapexRequest Not Found",
                    "NOT_FOUND",
                    $"Capex Request with id {id} Not found");
            }

            if (res.ApprovalStatus != "Submitted")
            {
                return ApiResponseHelper.Failure<string>(
                    "Invalid Request",
                    "INVALID_STATUS",
                    "Only Submitted Capex Requests can be Rejected");
            }

            var approvalFlow = await db.ApprovalFlows
                .FirstOrDefaultAsync(x =>
                    x.IsActive == 1 &&
                    res.Amount >= x.MinAmount &&
                    res.Amount <= x.MaxAmount);

            if (approvalFlow == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Approval Flow Not Configured",
                    "APPROVAL_FLOW_NOT_FOUND",
                    $"No Approval Flow configured for amount {res.Amount}");
            }

            if (approvalFlow.RequiredRoleId != roleId)
            {
                return ApiResponseHelper.Failure<string>(
                    "Unauthorized",
                    "ROLE_NOT_ALLOWED",
                    "Your role is not authorized to reject this CAPEX request.");
            }

            res.ApprovalStatus = "Rejected";
            res.ApprovedBy = userId;   // track who rejected (audit)
            res.ApprovedAt = DateTime.Now;
            res.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            memoryCache.Remove($"{id}");

            return ApiResponseHelper.SuccessRes<string>(
                $"Capex Request Rejected Successfully with id {id}");
        }

        public async Task<ApiResponse<List<CapexReqDTOGet>>> GetPendingApprovals(int userId)
        {
            var user = await db.Users
    .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return ApiResponseHelper.Failure<List<CapexReqDTOGet>>("User not found", "NOT_FOUND", "User not exist");
            }

            var roleId = user.RoleId;

            var pending = await (
                from c in db.CapexRequests.AsNoTracking()
                join af in db.ApprovalFlows.AsNoTracking()
                    on true equals true
                where c.ApprovalStatus == "Submitted"
                      && af.IsActive == 1
                      && af.RequiredRoleId == roleId
                      && c.Amount >= af.MinAmount
                      && c.Amount <= af.MaxAmount
                select c
            )
            .Distinct()
            .ProjectTo<CapexReqDTOGet>(map.ConfigurationProvider)
            .ToListAsync();

            if (pending == null || !pending.Any())
            {
                return ApiResponseHelper.Failure<List<CapexReqDTOGet>>(
                    "No Pending Approvals",
                    "EMPTY_DATA",
                    "There are no CAPEX requests awaiting your approval.");
            }

            return ApiResponseHelper.SuccessRes(
                pending,
                "Pending Approvals Fetched Successfully",
                pending.Count);
        }
    }
}
