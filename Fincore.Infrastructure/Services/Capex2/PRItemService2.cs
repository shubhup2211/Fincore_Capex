using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fincore.Application.DTO;
using Fincore.Application.DTO2;
using Fincore.Application.Interfaces.ICapex2;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Domain.Enums;

namespace Fincore.Infrastructure.Services.Capex2
{
    public class PRItemService2 : IPRItemService2
    {
        private readonly AppDbContext db;
        private readonly IMapper map;
        public PRItemService2(AppDbContext db, IMapper map) 
        {
            this.db = db;
            this.map = map;
        }

        public async Task<ApiResponse<string>> addPRItem(PRItemDTOPost2 dto)
        {
            var prId = await db.PurchaseRequisitions.FindAsync(dto.PurchaseRequisitionId);

            if (prId == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Purchase Requisition not found",
                    "PR_NOT_FOUND",
                    $"PR Id {dto.PurchaseRequisitionId} does not exist");
            }
            //pr active
            if (prId.IsActive==0)
            {
                return ApiResponseHelper.Failure<string>("Cannot add PRItem", "PR_INACTIVE", "Selected PR is inactive you cannot add any PRItem");
            }

            //pr must be draft
            if (prId.ApprovalStatus != ApprovalStatus.Draft.ToString())
            {
                return ApiResponseHelper.Failure<string>("Cannot add PRItem", "NOT_ALLOWED", "PR is already submitted you cannot add any PRItem");
            }

            var raise = map.Map<PurchaseRequisitionItem>(dto);
            raise.LineTotal = dto.Quantity * dto.EstimatedUnitPrice;
            await db.PurchaseRequisitionItems.AddAsync(raise);
            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes<string>($"PRItem added successfully for PR id {prId}");
            } else
            {
                return ApiResponseHelper.Failure<string>("PRItem add failed","ERROR_OCCURRED","Error occurred during add PRItem ");
            }
        }

        public async Task<ApiResponse<string>> deletePRItem(int prItemd)
        {
            var prItem = await db.PurchaseRequisitionItems.FindAsync(prItemd);
            if (prItem == null)
            {
                return ApiResponseHelper.Failure<string>("PRItem not exist", "NOT_EXIST", $"PRItem with Id {prItem} not exists");
            }

            var prId = await db.PurchaseRequisitions
                .Where(x=>x.PurchaseRequisitionId == prItem.PurchaseRequisitionId && x.ApprovalStatus==ApprovalStatus.Pending.ToString())
                .FirstOrDefaultAsync();
            if (prId == null)
            {
                return ApiResponseHelper.Failure<string>("PR not found", "NOT_Found", $"PR with Id {prId} either not exist or not in Pending status");
            }

            db.PurchaseRequisitionItems.Remove(prItem);
            int result = await db.SaveChangesAsync();

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes<string>($"PRItem deleted successfully for PR id {prId}");
            }
            else
            {
                return ApiResponseHelper.Failure<string>("PRItem delete failed", "ERROR_OCCURRED", "Error occurred during delete PRItem ");
            }
        }

        public async Task<ApiResponse<List<PRItemDTOGet2>>> getPRItemByPR(int prId)
        {
            var pr = await db.PurchaseRequisitions.FindAsync(prId);
            if (pr == null) 
            {
                return ApiResponseHelper.Failure<List<PRItemDTOGet2>>("PR Id not exist", "NOT_EXIST", $"PR Id {prId} not exists");
            }

            var prItem = await db.PurchaseRequisitionItems
                .Where(x=>x.PurchaseRequisitionId == prId)
                .ProjectTo<PRItemDTOGet2>(map.ConfigurationProvider)
                .ToListAsync();

            if(prItem.Count > 0)
            {
                return ApiResponseHelper.SuccessRes<List<PRItemDTOGet2>>(prItem, $"PR Item fetched successfully for PR id {prId}");               
            } else
            {
                return ApiResponseHelper.Failure<List<PRItemDTOGet2>>("No PR Item available", "NOT_FOUND", $"PR Item not exists for PR id {prId}");
            }

            
        }
    }
}
