using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.AutoMapper.Capex;
using Fincore.Application.DTO;
using Fincore.Application.DTO2;
using Fincore.Application.Interfaces.ICapex2;
using Fincore.Domain.Enums;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Services.Capex;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NPOI.Util.Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Infrastructure.Services.Capex2
{
    public class PRService2 : IPRService2
    {
        private readonly AppDbContext db;
        private readonly IMapper map;
        private readonly IMemoryCache memoryCache;
        private readonly LoginUser loginUser;
        public PRService2(AppDbContext db, IMapper map, IMemoryCache memoryCache, LoginUser loginUser) 
        { 
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
            this.loginUser = loginUser;
        }

        public async Task<ApiResponse<string>> raisePR(PRDTOPost2 dto)
        {
            int userId = loginUser.getUserId();
            string userRole = loginUser.getRoleName();

            //user login and role=PM check
            if (userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to raise a Purchase Requisition request");
            } 
            else if (!userRole.Equals("Procurement Head", StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponseHelper.Failure<string>("User not authenticated", "UNAUTHENTICATED", "User must be Procurement Manager to raise a Purchase Requisition request");
            }

                var checkCapex = await db.CapexRequests
                    .Include(x => x.BudgetLine)
                    .Where(x => x.CapexRequestId == dto.CapexRequestId && x.ApprovalStatus == ApprovalStatus.Approved.ToString())
                    .FirstOrDefaultAsync();

            //capex check
            if (checkCapex == null)
            {
                return ApiResponseHelper.Failure<string>("Capex request not found or not approved", "CAPEX_NOT_FOUND", $"The capex request with id {dto.CapexRequestId} does not exist or has not been approved.");
            }

            //pr amount check
            if (dto.Amount > checkCapex.Amount)
            {
                return ApiResponseHelper.Failure<string>("PR amount exceeds capex amount", "PR_AMOUNT_EXCEEDS_CAPEX", $"The PR amount of {dto.Amount} exceeds the approved capex amount of {checkCapex.Amount}.");
            }

            //required date check
            if (dto.RequiredTillDate < DateTime.Now)
            {
                return ApiResponseHelper.Failure<string>("Required date cannot be in the past", "REQUIRED_DATE_INVALID", $"The required date {dto.RequiredTillDate} cannot be in the past.");
            }

            //pr approver
            var financeManager = await db.Users
                .Where(x=> x.Role.RoleName.ToLower() == "Finance Manager")
                .FirstOrDefaultAsync();

            if (financeManager == null) 
            {
                return ApiResponseHelper.Failure<string>("Approver not found", "APPROVER_NOT_FOUND", "No user with the role of Finance Manager was found in the system.");
            }

            var raise = map.Map<PurchaseRequisition>(dto);
            raise.CategoryId = checkCapex.BudgetLine.VendorCategoryId;
            raise.PRNumber = getPRNumber();
            raise.RequestedBy = userId;
            raise.RequiredRoleId = financeManager.RoleId;
            raise.ApprovalStatus = ApprovalStatus.Draft.ToString();
            raise.CreatedAt = DateTime.UtcNow;
            raise.CreatedBy = userId;
            raise.ModifiedAt = DateTime.UtcNow; 
            raise.ModifiedBy = userId;
            raise.IsActive = 1;
            
            await db.PurchaseRequisitions.AddAsync(raise);
            int result = await db.SaveChangesAsync();
            memoryCache.Remove(raise.PurchaseRequisitionId);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes<string>(raise.PRNumber, "Purchase Requisition raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>("Failed to raise Purchase Requisition", "PR_RAISE_FAILED", "An error occurred while trying to raise the Purchase Requisition.");
            }


        }

        public async Task<ApiResponse<string>> approvePR(int prId)
        {
            int userId = loginUser.getUserId();
            int roleId = loginUser.getRoleId();

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User Not Authenticated", "UNAUTHENTICATED", "User must be logged in to see pending PR");
            }

            var PR = await db.PurchaseRequisitions
                .Include(x=>x.CapexRequest)
                .Where(x=>x.RequiredRoleId == roleId && x.ApprovalStatus==ApprovalStatus.Pending.ToString() && x.IsActive==1)
                .FirstOrDefaultAsync();

            if (PR == null) 
            {
                return ApiResponseHelper.Failure<string>("PR Not found", "NOT_FOUND", $"PR with id {prId} not found");
            }
            //capex is approved 
            if (PR.CapexRequest.ApprovalStatus != ApprovalStatus.Approved.ToString())
            {
                return ApiResponseHelper.Failure<string>("Capex Request not found", "NOT_FOUND", "Capex Request either get deleted or rejected");
            }
            //Pr amount < capex amt
            if(PR.Amount > PR.CapexRequest.Amount)
            {
                return ApiResponseHelper.Failure<string>("PR Amount exceed that Capex Amount", "INSUFFICIENT_BUDGET", $"PR amount {PR.Amount} exceeds the Capex Amount {PR.CapexRequest.Amount}");
            }

            PR.ApprovalStatus = ApprovalStatus.Approved.ToString();
            PR.ApprovedBy = userId;
            PR.ApprovedAt = DateTime.UtcNow;
            PR.CapexRequest.Amount -= PR.Amount ?? 0;

            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                memoryCache.Remove($"getAllPR_{userId}");
                memoryCache.Remove($"PendingPR_{userId}");
                return ApiResponseHelper.SuccessRes<string>(PR.PRNumber, "PR request approved successfully");
               
            }else
            {
                return ApiResponseHelper.Failure<string>("Failed to approve PR request", "FAILED_TO_APPROVE_PR", "An error occurred while approving the PR request");
            }           

        }

        public async Task<ApiResponse<string>> rejectPR(int prId)
        {
            int userId = loginUser.getUserId();
            int roleId = loginUser.getRoleId();

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User Not Authenticated", "UNAUTHENTICATED", "User must be logged in to see pending PR");
            }

            var pr  = await db.PurchaseRequisitions
                .Where(x=>x.RequiredRoleId== roleId && x.ApprovalStatus==ApprovalStatus.Pending.ToString() && x.IsActive==1)
                .FirstOrDefaultAsync();

            if (pr == null)
            {
                return ApiResponseHelper.Failure<string>("PR Not found", "NOT_FOUND", $"PR with id {prId} not found");
            }
            
            pr.ApprovalStatus = ApprovalStatus.Rejected.ToString();

            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                memoryCache.Remove($"getAllPR_{userId}");
                memoryCache.Remove($"PendingPR_{userId}");
                return ApiResponseHelper.SuccessRes<string>(pr.PRNumber, "PR request rejected successfully");

            }
            else
            {
                return ApiResponseHelper.Failure<string>("Failed to reject PR request", "FAILED_TO_REJECTE_PR", "An error occurred while rejectig the PR request");
            }
        }

        public async Task<ApiResponse<List<PRDTOGet2>>> getAllPR(int page, int pageSize, IsActive? Status,string? search)
        {
           int userId = loginUser.getUserId();
            string cacheKey = $"getAllPR_{userId}_{page}_{pageSize}_{Status}_{search}";

            if(userId == 0)
            {
                return ApiResponseHelper.Failure<List<PRDTOGet2>>("User not authenticated", "UNAUTHENTICATED", "User must be logged in to view Purchase Requisitions");
            }

            if (page <= 0 || pageSize <= 0)
            {
                return ApiResponseHelper.Failure<List<PRDTOGet2>>("Invalid pagination parameters", "INVALID_PAGINATION", "Page and pageSize must be greater than zero");
            }

            if(memoryCache.TryGetValue(cacheKey, out List<PRDTOGet2> prList))
            {
                return ApiResponseHelper.SuccessRes<List<PRDTOGet2>>(prList, "Purchase Requisitions retrieved successfully from cache", prList.Count);
            }

            IQueryable<PurchaseRequisition> query = db.PurchaseRequisitions.AsQueryable();

            if (Status.HasValue)
            {
                query = query.Where(x=> x.IsActive == (int)Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x=>x.PRNumber.Contains(search) || x.PRTitle.Contains(search));
            }
            prList = await query
                .OrderByDescending(x=>x.PurchaseRequisitionId)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ProjectTo<PRDTOGet2>(map.ConfigurationProvider).ToListAsync();

            memoryCache.Set(cacheKey, prList);

            if (prList.Count > 0)
            {
                return ApiResponseHelper.SuccessRes<List<PRDTOGet2>>(prList, "Purchase Requisitions retrieved successfully", prList.Count);
            }
            else
            {
                return ApiResponseHelper.Failure<List<PRDTOGet2>>("No Purchase Requisitions found", "NO_PR_FOUND", "There are no Purchase Requisitions matching the specified criteria");
            }
        }

        public async Task<ApiResponse<List<PRDTOGet2>>> getPendingPR(int page, int pageSize, string? search)
        {
            int userId = loginUser.getUserId();
            int roleId = loginUser.getRoleId();

            string cacheKey = $"PendingPR_{userId}_{page}_{pageSize}_{search}";

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<List<PRDTOGet2>>("User Not Authenticated", "UNAUTHENTICATED", "User must be logged in to see pending PR");
            }

            if (memoryCache.TryGetValue(cacheKey, out List<PRDTOGet2> prList))
            {
                return ApiResponseHelper.SuccessRes<List<PRDTOGet2>>(prList, "Pending PR fetched successfully", prList.Count);
            }

            IQueryable<PurchaseRequisition> query =  db.PurchaseRequisitions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query =  query.Where(x=>x.PRNumber.Contains(search) || x.PRTitle.Contains(search));
            }

            prList = await query
                .Where(x=>x.RequiredRoleId == roleId && x.ApprovalStatus == ApprovalStatus.Pending.ToString() && x.IsActive==1)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ProjectTo<PRDTOGet2>(map.ConfigurationProvider).ToListAsync();

            memoryCache.Set(cacheKey, prList);

            if (prList.Count > 0)
            {
                return ApiResponseHelper.SuccessRes<List<PRDTOGet2>>(prList, "Pending PR fetched successfully", prList.Count);
            }
            else {
                return ApiResponseHelper.Failure<List<PRDTOGet2>>("No Purchase Requisitions found", "NO_PR_FOUND", "There are no Purchase Requisitions matching the specified criteria");
            }
        }

        public Task<ApiResponse<PRDTOGet2>> getPRByUser(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> submitPR(int prId)
        {
            int userId = loginUser.getUserId();
            int roleId = loginUser.getRoleId();

            if (userId == 0)
            {
                return ApiResponseHelper.Failure<string>("User Not Authenticated", "UNAUTHENTICATED", "User must be logged in to see pending PR");
            }

            var pr = await db.PurchaseRequisitions
                .Where(x=>x.PurchaseRequisitionId == prId && x.ApprovalStatus== ApprovalStatus.Draft.ToString() && x.IsActive==1)
                .FirstOrDefaultAsync();

            //pr check
            if (pr == null)
            {
                return ApiResponseHelper.Failure<string>("PR Not found", "NOT_FOUND", $"PR with id {prId} not found");
            }

            //pritem exist or not
            var prItem = await db.PurchaseRequisitionItems
                .Where(x => x.PurchaseRequisitionId == prId)
                .FirstOrDefaultAsync();

            if(prItem == null)
            {
                return ApiResponseHelper.Failure<string>($"PRItem Not found with PR id {prId}", "NOT_FOUND", "At least one PR Item must be exist to submit PR");
            }

            pr.ApprovalStatus = ApprovalStatus.Pending.ToString();
            pr.ModifiedAt = DateTime.UtcNow;
            pr.ModifiedBy = userId;

            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                memoryCache.Remove($"getAllPR_{userId}");
                memoryCache.Remove($"PendingPR_{userId}");
                return ApiResponseHelper.SuccessRes<string>(pr.PRNumber, "PR request submitted successfully");

            }
            else
            {
                return ApiResponseHelper.Failure<string>("Failed to submit PR request", "FAILED_TO_Submit_PR", "An error occurred while submitting the PR request");
            }

        }
        private string getPRNumber()
        {
            int currentYear = DateTime.Now.Year;

            var pr = db.PurchaseRequisitions
                .Where(x=> x.PRNumber.StartsWith($"PR-{currentYear}-"))
                .OrderByDescending(x=> x.PRNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;

            if(pr != null)
            {
                nextSequence = int.Parse(pr.Result.PRNumber.Split('-')[2]) + 1;
            }

            string prNumber = $"PR-{currentYear}-{nextSequence:D4}";

            return prNumber;
        }


    }
}
