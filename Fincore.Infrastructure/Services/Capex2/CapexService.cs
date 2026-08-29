using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using Fincore.Application.DTO2;
using Fincore.Application.DTOs.BudgetLine;
using Fincore.Application.Interfaces.ICapex2;
using Fincore.Domain.Enums;
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

namespace Fincore.Infrastructure.Services.Capex2
{
    public class CapexService : ICapexService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        LoginUser loginUser;
        public CapexService(AppDbContext db, IMapper mapper, IMemoryCache memoryCache, LoginUser loginUser) { 
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
            this.loginUser = loginUser;
        }


        public async Task<ApiResponse<string>> RaiseCapex(CapexDTOPost dto)
        {
            int userId = loginUser.getUserId();
            int userDepartmentId = loginUser.getDepartmentId();

            if(userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to raise a capex request");
            }
          
            var budgetLine = await db.BudgetLines.FindAsync(dto.BudgetLineId);

            //budgetline exists check
            if (budgetLine == null)
            {
                return ApiResponseHelper.Failure<string>("Budget line not found","NOT_FOUND", $"Budget line ID {dto.BudgetLineId} not exist");
            }

            //user & budgetline department check
            if (userDepartmentId != budgetLine.DepartmentId)
            {
                return ApiResponseHelper.Failure<string>("User department and BudgetLine department mismatch", "DEPARTMENT_MISMATCH", $"You are not authorized to use this budgetlineId");
            }
            //butdgetline active
            if (budgetLine.IsActive==0)
            {
                return ApiResponseHelper.Failure<string>("Budget line is not active", "INACTIVE_BUDGETLINE", $"Budget line ID {dto.BudgetLineId} is not active");
            }
            //budgetline remaining budget
            if ( dto.Amount > budgetLine.RemainingAmount)
            {
                return ApiResponseHelper.Failure<string>("Insufficient budget", "INSUFFICIENT_BUDGET", $"Requested amount {dto.Amount} exceeds remaining budget {budgetLine.RemainingAmount}");
            }

            //Approval flow check
            var approvalFLow = await db.ApprovalFlows
                .Where(x=> x.IsActive==1 && dto.Amount >=x.MinAmount && dto.Amount <= x.MaxAmount)
                .FirstOrDefaultAsync();
            if (approvalFLow == null)
            {
                return ApiResponseHelper.Failure<string>("No approval flow found for the requested amount", "NO_APPROVAL_FLOW", $"No active approval flow found for the requested amount {dto.Amount}");
            }

            var rolename = (await db.Roles.FindAsync(approvalFLow.RequiredRoleId))?.RoleName;

            var approver = 0;

            //actual approver
            if(rolename.Equals("Department Head", StringComparison.OrdinalIgnoreCase) ||
                rolename.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                var manager = (await db.Departments.FindAsync(userDepartmentId))?.ManagerId ?? 0; 

                var managerUserId = await db.Employees
                    .Where(x=>x.EmployeeId == manager)
                    .Select(x=>x.UserId).FirstOrDefaultAsync();

                approver = managerUserId;
                
            } 
            else
            {
                var employee = await db.Employees
                    .FirstOrDefaultAsync(x => x.Designation == approvalFLow.RequiredRoleId);

                approver = employee?.UserId ?? 0;
            }

            var raise = mapper.Map<CapexRequest>(dto);
            raise.CapexReqNumber = GenerateCapexNumber();
            raise.RequestedBy = userId;
            raise.CreatedAt = DateTime.UtcNow;
            raise.ApprovalStatus = ApprovalStatus.Pending.ToString();
            raise.RequiredRoleId = approvalFLow.RequiredRoleId;
            raise.ApproverId = approver;
            await db.CapexRequests.AddAsync(raise);
            int result = await db.SaveChangesAsync();
            memoryCache.Remove(raise.CapexRequestId);

            if(result > 0)
            {
                return ApiResponseHelper.SuccessRes<string>(raise.CapexReqNumber, "Capex request raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>("Failed to raise capex request", "FAILED_TO_RAISE_CAPEX", "An error occurred while raising the capex request");
            }

        }

        public async Task<ApiResponse<string>> ApproveCapex(int id)
        {
            int userId = loginUser.getUserId();

            if(userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to approve a capex request");
            }

            var capexRequest = await db.CapexRequests
                .Include(x => x.BudgetLine)
                .Where(x => x.CapexRequestId == id && x.ApproverId == userId && x.ApprovalStatus == ApprovalStatus.Pending.ToString())
                .FirstOrDefaultAsync();


            if(capexRequest == null)
            {
                return ApiResponseHelper.Failure<string>("Capex request not found", "NOT_FOUND", $"Capex request ID {id} not found");
            }

            if(capexRequest.BudgetLine.IsActive == 0)
            {
                return ApiResponseHelper.Failure<string>("Cannot approve this capex", "INACTIVE_BUDGETLINE", $"Cannot Approve Capex because BudgetLine ID {capexRequest.BudgetLineId} is not active");
            }

            if(capexRequest.Amount > capexRequest.BudgetLine.RemainingAmount)
            {
                return ApiResponseHelper.Failure<string>("Cannot approve this capex", "INSUFFICIENT_BUDGET", $"Cannot Approve Capex because Requested amount {capexRequest.Amount} exceeds remaining budget");
            }

            capexRequest.ApprovalStatus = ApprovalStatus.Approved.ToString();
            capexRequest.ApprovedBy = loginUser.getUserId();
            capexRequest.ApprovedAt = DateTime.UtcNow;
            capexRequest.BudgetLine.UtilizedAmount += capexRequest.Amount;
            capexRequest.BudgetLine.RemainingAmount -= capexRequest.Amount;

            int result = await db.SaveChangesAsync();

            if(result <= 0)
            {
               return ApiResponseHelper.Failure<string>("Failed to approve capex request", "FAILED_TO_APPROVE_CAPEX", "An error occurred while approving the capex request");
            }
            return ApiResponseHelper.SuccessRes<string>(capexRequest.CapexReqNumber, "Capex request approved successfully");
        }

        public async Task<ApiResponse<string>> RejectCapex(int id)
        {
            int userId = loginUser.getUserId();
            if(userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to reject a capex request");
            }

            var capexRequest = await db.CapexRequests
                .Where(x => x.CapexRequestId == id && x.ApproverId == userId && x.ApprovalStatus == ApprovalStatus.Pending.ToString())
                .FirstOrDefaultAsync();

            if(capexRequest == null)
            {
                return ApiResponseHelper.Failure<string>("Capex request not found", "NOT_FOUND", $"Capex request ID {id} not found");
            }

            capexRequest.ApprovalStatus = ApprovalStatus.Rejected.ToString();

            int result = await db.SaveChangesAsync();

            if (result <= 0)
            {
                return ApiResponseHelper.Failure<string>("Failed to reject capex request", "FAILED_TO_REJECT_CAPEX", "An error occurred while rejecting the capex request");
            }
            return ApiResponseHelper.SuccessRes<string>(capexRequest.CapexReqNumber, "Capex request rejected successfully");

        }

        public async Task<ApiResponse<List<CapexDTOGet>>> GetAllCapex(int page, int pageSize)
        {
            string cacheKey = $"CapexRequests_Page_{page}_PageSize_{pageSize}";

            if (memoryCache.TryGetValue(cacheKey, out List<CapexDTOGet> capexList))
            {
                return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>(capexList, "Capex requests retrieved successfully from cache", capexList.Count);
            }

            if (page < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page number.", "INVALID_PAGE", "Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page size.", "INVALID_PAGE_SIZE", "Page size must be greater than or equal to 1.");
            }
            capexList = await db.CapexRequests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CapexDTOGet>(mapper.ConfigurationProvider).ToListAsync();

            if (capexList.Count == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("No capex requests found", "NO_CAPEX_FOUND", "There are no capex requests available");
            }

            memoryCache.Set(cacheKey, capexList);

            return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>(capexList, "Capex requests retrieved successfully from cache", capexList.Count);

        }

        public async Task<ApiResponse<List<CapexDTOGet>>> GetCapexByUserId(int page, int pageSize)
        {
            int userId = loginUser.getUserId();
            string cacheKey = $"CapexRequests_User_{userId}_Page_{page}_PageSize_{pageSize}";

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to view a capex request");
            }

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to view a capex request");
            }

            if (memoryCache.TryGetValue(cacheKey, out List<CapexDTOGet> capexList))
            {
                return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>(capexList, "Capex requests retrieved successfully from cache", capexList.Count);
            }

            if (page < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page number.", "INVALID_PAGE", "Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page size.", "INVALID_PAGE_SIZE", "Page size must be greater than or equal to 1.");
            }

             capexList = await db.CapexRequests
                .Where(x => x.RequestedBy == userId)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .OrderByDescending(x => x.CreatedAt)
                .ProjectTo<CapexDTOGet>(mapper.ConfigurationProvider).ToListAsync();

            if (capexList.Count == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("No capex requests found for the user", "NO_CAPEX_FOUND", $"User ID {userId} has not raised any capex requests");
            }
            else
            {
                return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>(capexList, "Capex requests retrieved successfully",capexList.Count);
            }
        }

        public async Task<ApiResponse<List<CapexDTOGet>>> GetPendingCapex(int page, int pageSize)
        {
            int userId = loginUser.getUserId();
            if (userId == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to view pending capex requests");
            }

            string cacheKey = $"CapexRequests_User_{userId}_Page_{page}_PageSize_{pageSize}";

            if (memoryCache.TryGetValue(cacheKey, out List<CapexDTOGet> pendingCapexList))
            {
                return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>(pendingCapexList, "Pending Capex retrieved successfully from cache", pendingCapexList.Count);
            }

            if (page < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page number.", "INVALID_PAGE", "Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>(
                    "Invalid page size.", "INVALID_PAGE_SIZE", "Page size must be greater than or equal to 1.");
            }

            pendingCapexList = await db.CapexRequests
                .Where(x=> x.ApproverId == userId && x.ApprovalStatus == ApprovalStatus.Pending.ToString())
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ProjectTo<CapexDTOGet>(mapper.ConfigurationProvider).ToListAsync();

            if (pendingCapexList.Count == 0)
            {
                return ApiResponseHelper.Failure<List<CapexDTOGet>>("No pending capex requests found for the user", "NO_PENDING_CAPEX_FOUND", $"User ID {userId} has no pending capex requests");
            }

            return ApiResponseHelper.SuccessRes<List<CapexDTOGet>>( pendingCapexList, "Pending capex requests retrieved successfully", pendingCapexList.Count);

        }

        //Generate Capex numer
        private string GenerateCapexNumber()
        {
            int year = DateTime.UtcNow.Year;

            var lastCapex = db.CapexRequests
                .Where(x => x.CapexReqNumber.StartsWith($"CAPEX-{year}-"))
                .OrderByDescending(x => x.CapexReqNumber)
                .FirstOrDefaultAsync();

            int next = 1;
            if (lastCapex != null)
            {
                next = int.Parse(lastCapex.Result.CapexReqNumber.Split('-')[2]) + 1;
            }

            string capexNumber = $"CAPEX-{year}-{next:D3}";

            return capexNumber;
        }

        public async Task<ApiResponse<List<BudgetLineResponseDTO>>> GetBudgetLinesByDepartment()
        {
            int departmentId = loginUser.getDepartmentId();
            var budgetLines = await db.BudgetLines
                .Where(x => x.DepartmentId == departmentId && x.IsActive == 1)
                .Select(x => new BudgetLineResponseDTO
                {
                    BudgetLineId = x.BudgetLineId         })
                .OrderBy(x => x.BudgetLineId)
                .ToListAsync();

            if (!budgetLines.Any())
            {
                return ApiResponseHelper.Failure<List<BudgetLineResponseDTO>>(
                    "No budget lines found",
                    "EMPTY_DATA",
                    $"No budget lines available for Department {departmentId}");
            }

            return ApiResponseHelper.SuccessRes(budgetLines);
        }
    }
}
