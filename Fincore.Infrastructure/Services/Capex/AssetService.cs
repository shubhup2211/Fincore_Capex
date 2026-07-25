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
    public class AssetService : IAssetService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;

        private const string AssetCacheKey = "Asset";


        public AssetService(
            AppDbContext db,
            IMapper mapper,
            IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }



        public async Task<ApiResponse<AssetDTO>> AddAsset(AssetDTO dto)
        {

            var exists = await db.Assets
                .AnyAsync(x => x.AssetCode == dto.AssetCode);


            if (exists)
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Code Already Exists",
                    "400",
                    "Duplicate Asset Code"
                );
            }



            if (dto.PurchaseCost <= 0)
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Invalid Cost",
                    "400",
                    "Purchase cost must be greater than zero"
                );
            }



            if (dto.GRNId != null)
            {
                var grnExists = await db.GRNs
                    .AnyAsync(x => x.GRNId == dto.GRNId);


                if (!grnExists)
                {
                    return ApiResponseHelper.Failure<AssetDTO>(
                        "GRN Not Found",
                        "404",
                        "Invalid GRN"
                    );
                }
            }



            var asset = mapper.Map<Asset>(dto);


            asset.Status = "AVAILABLE";
            asset.CreatedAt = DateTime.Now;


            await db.Assets.AddAsync(asset);

            await db.SaveChangesAsync();


            cache.Remove(AssetCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(asset),
                "Asset Created Successfully"
            );
        }



        public async Task<ApiResponse<AssetDTO>> GetAsset(int id)
        {
            var data = await db.Assets
                               .FirstOrDefaultAsync(x => x.AssetId == id);


            if (data == null)
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Not Found",
                    "404",
                    "Record not found"
                );
            }


            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(data),
                "Asset Retrieved Successfully"
            );
        }



        public async Task<ApiResponse<List<AssetDTO>>> GetAllAssets(
            int page,
            int pageSize)
        {
            string cacheKey = $"{AssetCacheKey}_{page}_{pageSize}";


            if (cache.TryGetValue(cacheKey, out List<AssetDTO> assets))
            {
                var totalRecords = await db.Assets.CountAsync();

                return ApiResponseHelper.SuccessRes(
                    assets,
                    "Assets Retrieved Successfully",
                    totalRecords,
                    new
                    {
                        page,
                        pageSize
                    }
                );
            }



            var data = await db.Assets
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();



            assets = mapper.Map<List<AssetDTO>>(data);



            if (!assets.Any())
            {
                return ApiResponseHelper.Failure<List<AssetDTO>>(
                    "Assets Not Found",
                    "ASSET_NOT_FOUND",
                    "No Asset records found"
                );
            }



            cache.Set(
                cacheKey,
                assets,
                TimeSpan.FromMinutes(5)
            );



            var totalRecord = await db.Assets.CountAsync();



            return ApiResponseHelper.SuccessRes(
                assets,
                "Assets Retrieved Successfully",
                totalRecord,
                new
                {
                    page,
                    pageSize
                }
            );
        }




        public async Task<ApiResponse<AssetDTO>> UpdateAsset(
            int id,
            AssetDTO dto)
        {
            var data = await db.Assets
                               .FirstOrDefaultAsync(x => x.AssetId == id);



            if (data == null)
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Not Found",
                    "404",
                    "Record not found"
                );
            }



            mapper.Map(dto, data);



            await db.SaveChangesAsync();



            cache.Remove(AssetCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(data),
                "Asset Updated Successfully"
            );
        }




        public async Task<ApiResponse<AssetDTO>> DeleteAsset(int id)
        {

            var asset = await db.Assets
                .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Not Found",
                    "404",
                    "Record not found"
                );
            }



            if (asset.Status == "ASSIGNED")
            {
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Cannot Delete",
                    "400",
                    "Assigned Asset cannot be deleted"
                );
            }



            asset.Status = "DELETED";
            asset.ModifiedAt = DateTime.Now;



            await db.SaveChangesAsync();


            cache.Remove(AssetCacheKey);



            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(asset),
                "Asset Deleted Successfully"
            );
        }

        public async Task<ApiResponse<AssetDTO>> AssignAsset(int id, int userId)
        {

            var asset = await db.Assets
                .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Not Found",
                    "404",
                    "Invalid Asset"
                );


            if (asset.Status == "DISPOSED")
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Cannot Assign",
                    "400",
                    "Disposed Asset cannot assign"
                );


            asset.Status = "ASSIGNED";
            asset.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(asset),
                "Asset Assigned Successfully"
            );

        }

        public async Task<ApiResponse<AssetDTO>> TransferAsset(int id, int departmentId)
        {

            var asset = await db.Assets
                .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Asset Not Found",
                    "404",
                    "Invalid Asset"
                );


            var deptExists = await db.Departments
                .AnyAsync(x => x.DepartmentId == departmentId);


            if (!deptExists)
                return ApiResponseHelper.Failure<AssetDTO>(
                    "Department Not Found",
                    "404",
                    "Invalid Department"
                );


            asset.DepartmentId = departmentId;
            asset.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
                mapper.Map<AssetDTO>(asset),
                "Asset Transferred Successfully"
            );

        }

        public async Task<ApiResponse<AssetDTO>> DisposeAsset(int id)
        {

            var asset = await db.Assets
            .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
                return ApiResponseHelper.Failure<AssetDTO>(
                "Asset Not Found", "404", "Invalid Asset");


            asset.Status = "DISPOSED";
            asset.ModifiedAt = DateTime.Now;


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
            mapper.Map<AssetDTO>(asset),
            "Asset Disposed Successfully");

        }

        public async Task<ApiResponse<AssetDTO>> RepairAsset(int id)
        {

            var asset = await db.Assets
            .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
                return ApiResponseHelper.Failure<AssetDTO>(
                "Asset Not Found", "404", "Invalid Asset");


            asset.Status = "REPAIR";


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
            mapper.Map<AssetDTO>(asset),
            "Asset Sent For Repair");

        }

        public async Task<ApiResponse<AssetDTO>> ReturnAsset(int id)
        {

            var asset = await db.Assets
            .FirstOrDefaultAsync(x => x.AssetId == id);


            if (asset == null)
                return ApiResponseHelper.Failure<AssetDTO>(
                "Asset Not Found", "404", "Invalid Asset");


            asset.Status = "AVAILABLE";


            await db.SaveChangesAsync();


            return ApiResponseHelper.SuccessRes(
            mapper.Map<AssetDTO>(asset),
            "Asset Returned Successfully");

        }
    }
}