using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Enums;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Fincore.Infrastructure.Services.MasterTable
{
    public class AccountMasterService : IAccountMasterService
    {
        AppDbContext db;
        IMapper mapper;
        IMemoryCache cache;
        
        

        public AccountMasterService(AppDbContext db, IMapper mapper, IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.cache = cache;
        }
        public async Task<ApiResponse<AccountMasterPostDTO>> AddAccountsMaster(AccountMasterPostDTO dto, int count)
        {
            
            var data = mapper.Map<AccountMaster>(dto);
            data.AccountCode = (2000 + count).ToString();
            data.CreatedAt = DateTime.Now;
            data.ModifiedAt = DateTime.Now;
            data.CreatedBy = 1;
            data.ModifiedBy = 1;
            
            await db.AccountMasters.AddAsync(data);
            await db.SaveChangesAsync();
            return ApiResponseHelper.SuccessRes(
                dto,
                "Account created successfully!"
                );
        }

        public async Task<ApiResponse<bool>> DeleteAccount(int id)
        {
            var data = await db.AccountMasters
                .FirstOrDefaultAsync(x => x.AccountId == id);

            if (data == null)
            {
                return ApiResponseHelper.Failure<bool>(
                    "Account not found.",
                    "ACCOUNT_NOT_FOUND",
                    $"No account found with Id {id}."
                );
            }

            db.AccountMasters.Remove(data);
            await db.SaveChangesAsync();

            return ApiResponseHelper.SuccessRes(
                true,
                "Account deleted successfully!"
            );
        }

        public async Task<ApiResponse<AccountMasterGetDTO>> GetAccountById(int id)
        {
            var data = await db.AccountMasters.FirstOrDefaultAsync(x => x.AccountId == id);
            if(data == null)
            {
                return ApiResponseHelper.Failure<AccountMasterGetDTO>(
                    "Account not found.",
                    "ACCOUNT_NOT_FOUND",
                    $"No account found with Id {id}."
                );
            }
            var result = mapper.Map<AccountMasterGetDTO>(data);
            return ApiResponseHelper.SuccessRes(result,
                "Fetching data by ID",
                await db.AccountMasters.CountAsync()
                );


        }

        public async Task<ApiResponse<List<AccountMasterGetDTO>>> GetAccountType(AccountType type, int page, int pageSize)
        {
            string AccType = type.ToString();
            string cacheKey = $"AccountType_Accounts_{page}_{pageSize}";
            if(cache.TryGetValue(cacheKey, out List<AccountMasterGetDTO> accounts))
            {
                return ApiResponseHelper.SuccessRes(accounts, "Accounts Fetched Successfully", await db.AccountMasters.Where(x => x.AccountType == AccType).CountAsync()); 

            }

            var data = await db.AccountMasters.Where(x=>x.AccountType==AccType).OrderBy(x=>x.AccountId).Skip((page-1)* pageSize).Take(pageSize).ToListAsync();
            var result = mapper.Map<List<AccountMasterGetDTO>>(data);
            return ApiResponseHelper.SuccessRes(result, "Accounts Fetched Successfully", await db.AccountMasters.Where(x => x.AccountType == AccType).CountAsync());

        }

        public async Task<ApiResponse<List<AccountMasterGetDTO>>> GetActiveAccounts(int page, int pageSize)
        {
            string cacheKey = $"Active_Accounts_{page}_{pageSize}";
            if(cache.TryGetValue(cacheKey, out List<AccountMasterGetDTO> accounts))
            {
                return ApiResponseHelper.SuccessRes(accounts,"Active Accounts Fetched Successfully!", await db.AccountMasters.CountAsync() );
            }

            var data = await db.AccountMasters
    .Where(x => x.IsActive == 1)
    .OrderBy(x => x.AccountId)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
            var result = mapper.Map<List<AccountMasterGetDTO>>(data);
            cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return ApiResponseHelper.SuccessRes(result,
                "Active accounts fetched successfully!",
                await db.AccountMasters
    .CountAsync(x => x.IsActive == 1)
                );

        }

        public async Task<ApiResponse<List<AccountMasterGetDTO>>> GetAllAccounts(int page, int pageSize)
        {
            string cacheKey = $"Accounts_{page}_{pageSize}";

            if (cache.TryGetValue(cacheKey, out List<AccountMasterGetDTO> accounts))
            {
                return ApiResponseHelper.SuccessRes(
                    accounts,
                    "Accounts fetched successfully!",
                    await db.AccountMasters.CountAsync()
                    );
            }

            var data = await db.AccountMasters.OrderBy(x=>x.AccountId).Skip((page - 1)* pageSize).Take(pageSize).ToListAsync();

            var result = mapper.Map<List<AccountMasterGetDTO>>(data);
            cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return ApiResponseHelper.SuccessRes(
                result,
                "Accounts fetched successfully!",
                await db.AccountMasters.CountAsync()
                );
            
        }

        

        public async Task<int> GetCount()
        {
            return await db.AccountMasters.CountAsync();
        }

        public async Task<ApiResponse<List<AccountMasterGetDTO>>> GetPendingAccounts(int page, int pageSize)
        {
            string cacheKey = $"Pending_Accounts_{page}_{pageSize}";
            if(cache.TryGetValue(cacheKey, out List<AccountMasterGetDTO> accounts))
            {
                return ApiResponseHelper.SuccessRes(accounts, "Pending Accounts Fetched", await  db.AccountMasters.Where(x=>x.IsActive==0).CountAsync() );
            }
            var data = await db.AccountMasters.Where(x=>x.IsActive==0).OrderBy(x=>x.AccountId).Skip((page-1)* pageSize).Take(pageSize).ToListAsync();

            var result = mapper.Map<List<AccountMasterGetDTO>>(data);

            return ApiResponseHelper.SuccessRes(result, "Pending Accounts Fetched", await db.AccountMasters.Where(x => x.IsActive == 0).CountAsync());



        }

        public async Task<ApiResponse<AccountMasterGetDTO>> UpdateAccount(int id, AccountMasterPutDTO dto)
        {
            var data = await db.AccountMasters
                .FirstOrDefaultAsync(x => x.AccountId == id);

            if (data == null)
            {
                return ApiResponseHelper.Failure<AccountMasterGetDTO>(
                    "Account not found.",
                    "ACCOUNT_NOT_FOUND",
                    $"No account found with Id {id}."
                );
            }

            data.AccountName = dto.AccountName;
            data.AccountType = dto.AccountType;
            data.IsActive = dto.IsActive;

            data.ModifiedAt = DateTime.Now;
            data.ModifiedBy = 1;

            await db.SaveChangesAsync();

            var result = mapper.Map<AccountMasterGetDTO>(data);

            return ApiResponseHelper.SuccessRes(
                result,
                "Account updated successfully!"
            );
        }
    }
}
