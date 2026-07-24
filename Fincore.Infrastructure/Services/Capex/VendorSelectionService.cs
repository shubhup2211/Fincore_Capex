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
    public class VendorSelectionService : IVendorSelectionService
    {
        private readonly AppDbContext db;
        IMapper map;
        IMemoryCache memoryCache;

        public VendorSelectionService(AppDbContext db, IMapper map, IMemoryCache memoryCache)
        {
            this.db = db;
            this.map = map;
            this.memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> CreateVendorSelection(VendorSelectionDTOPost vendorSelection)
        {
            var add = map.Map<VendorSelection>(vendorSelection);
            
            await db.VendorSelections.AddAsync(add);
            var result = await db.SaveChangesAsync();
            memoryCache.Remove(vendorSelection);

            if (result > 0)
            {
                return ApiResponseHelper.SuccessRes(
                    result.ToString(), "Vendor Selection raised successfully");
            }
            else
            {
                return ApiResponseHelper.Failure<string>(
                    "Vendor Selection failed", "ERROR_OCCURED", "Failed to raise Vendor Selection");
            }
        }

        public async Task<ApiResponse<string>> DeleteVendorSelection(int id)
        {
            var vendorSelection = await db.VendorSelections.FindAsync(id);

            if (vendorSelection == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Cannot Delete Vendor Selection", "NOT_FOUND", $"Vendor Selection with id {id} Not found");
            }

            db.VendorSelections.Remove(vendorSelection);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes($"Vendor Selection Deleted Successfully with id {id}");
        }

        public async Task<ApiResponse<List<VendorSelectionDTOGet>>> GetVendorSelection(int page, int pagesize)
        {
            string cacheKey = $"VendorSelection_{page}_{pagesize}";

            if (memoryCache.TryGetValue(cacheKey, out List<VendorSelectionDTOGet> VendorSelectionlist))
            {
                return ApiResponseHelper.SuccessRes(
                    VendorSelectionlist,
                    "Vendor Selections fetched successfully",
                    VendorSelectionlist.Count,
                    new { page = page, pagesize = pagesize });
            }

            VendorSelectionlist = await db.VendorSelections
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ProjectTo<VendorSelectionDTOGet>(map.ConfigurationProvider)
                .ToListAsync();

            if (VendorSelectionlist == null & !VendorSelectionlist.Any())
            {
                return ApiResponseHelper.Failure<List<VendorSelectionDTOGet>>(
                    "Vendor Selections not found",
                    "EMPTY_DATA",
                    "No Data to show");
            }

            memoryCache.Set(cacheKey, VendorSelectionlist);

            return ApiResponseHelper.SuccessRes(
                VendorSelectionlist,
                "Vendor Selections fetched successfully",
                VendorSelectionlist.Count,
                new { page = page, pagesize = pagesize });
        }

        public async Task<ApiResponse<VendorSelectionDTOGet>> GetVendorSelectionById(int id)
        {
            string cacheKey = $"{id}";

            if (memoryCache.TryGetValue(cacheKey, out VendorSelectionDTOGet vendorSelection))
            {
                return ApiResponseHelper.SuccessRes(
                    vendorSelection,
                    $"Vendor Selection with Id {id} found",
                    1);
            }

            vendorSelection = await db.VendorSelections
                .Where(x => x.VendorSelectionId == id)
                .ProjectTo<VendorSelectionDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (vendorSelection == null)
            {
                return ApiResponseHelper.Failure<VendorSelectionDTOGet>(
                    "Vendor Selection Record not found",
                    "NOT_FOUND",
                    $"Vendor Selection with Id {id} not found");
            }

            return ApiResponseHelper.SuccessRes(
                vendorSelection,
                $"Vendor Selection with Id {id} found",
                1);
        }

        public async Task<ApiResponse<string>> UpdateVendorSelection(int id, VendorSelectionDTOPost vendorSelection)
        {
            var update = await db.VendorSelections.FirstOrDefaultAsync(x => x.VendorSelectionId == id);

            if (update == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Vendor Selection Not Found",
                    "NOT_FOUND",
                    $"Vendor Selection with id {id} Not found");
            }

            

            map.Map(vendorSelection, update);
            await db.SaveChangesAsync();

            string cache = $"{id}";
            memoryCache.Remove(cache);

            return ApiResponseHelper.SuccessRes($"Vendor Selection Updated Successfully with id {id}");
        }

        public async Task<ApiResponse<List<QuotationComparisonDTO>>> CompareQuotations(int rfqId)
        {
            var quotations = await db.Quotations
                .Where(x => x.RFQId == rfqId)
                .Select(x => new QuotationComparisonDTO
                {
                    QuotationId = x.QuotationId,
                    QuotationNumber = x.QuotationNumber,
                    CompanyName = x.Vendor.Company.CompanyName,
                    VendorCode = x.Vendor.VendorCode,
                    QuotedAmount = x.QuotedAmount,
                    Remarks = x.Remarks,
                    SelectionStatus = x.IsSelected == 1 ? "Selected" : "Not Selected",
                    CreatedAt = x.CreatedAt
                })
                .OrderBy(x => x.QuotedAmount)
                .ToListAsync();

            if (!quotations.Any())
            {
                return ApiResponseHelper.Failure<List<QuotationComparisonDTO>>(
                    "No Quotations Found",
                    "EMPTY_DATA",
                    "No quotations found for this RFQ.");
            }

            return ApiResponseHelper.SuccessRes(
                quotations,
                "Quotation comparison fetched successfully",
                quotations.Count);
        }

        public async Task<ApiResponse<string>> SelectVendor(int rfqId, int vendorId)
        {
            var rfq = await db.RFQs
                .FirstOrDefaultAsync(x => x.RFQId == rfqId);

            if (rfq == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "RFQ Not Found",
                    "NOT_FOUND",
                    $"RFQ with id {rfqId} not found");
            }

            var quotation = await db.Quotations
                .FirstOrDefaultAsync(x => x.RFQId == rfqId && x.VendorId == vendorId);

            if (quotation == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Quotation Not Found",
                    "NOT_FOUND",
                    "Vendor has not submitted a quotation.");
            }

            bool alreadySelected = await db.VendorSelections
                .AnyAsync(x => x.RFQId == rfqId);

            if (alreadySelected)
            {
                return ApiResponseHelper.Failure<string>(
                    "Vendor Already Selected",
                    "ALREADY_SELECTED",
                    "Vendor has already been selected.");
            }

            var quotations = await db.Quotations
                .Where(x => x.RFQId == rfqId)
                .ToListAsync();

            foreach (var item in quotations)
            {
                item.IsSelected = 0;
            }

            quotation.IsSelected = 1;

            var vendorSelection = new VendorSelection
            {
                RFQId = rfqId,
                QuotationId = quotation.QuotationId,
                SelectedVendorId = vendorId,
                SelectedDate = DateTime.Now,
                SelectedBy = rfq.CreatedBy,
                Remarks = "Vendor Selected"
            };

            await db.VendorSelections.AddAsync(vendorSelection);

            await db.SaveChangesAsync();

            memoryCache.Remove($"{rfqId}");

            return ApiResponseHelper.SuccessRes(
                "Vendor Selected Successfully");
        }

        public async Task<ApiResponse<VendorSelectionDTOGet>> GetSelectedVendorByRFQ(int rfqId)
        {
            var vendor = await db.VendorSelections
                .Where(x => x.RFQId == rfqId)
                .ProjectTo<VendorSelectionDTOGet>(map.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                return ApiResponseHelper.Failure<VendorSelectionDTOGet>(
                    "Vendor Selection Not Found",
                    "NOT_FOUND",
                    $"No vendor selected for RFQ {rfqId}");
            }

            return ApiResponseHelper.SuccessRes(
                vendor,
                "Selected Vendor fetched successfully",
                1);
        }
    }
}