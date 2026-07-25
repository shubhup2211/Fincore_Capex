using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTOs.ExpenseClaim;
using Fincore.Application.Interfaces.ExpenseClaim;
using ExpenseClaimModel = Fincore.Domain.Models.ExpenseClaim;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services.ExpenseClaim
{ 
    public class ExpenseClaimService : IExpenseClaimService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ExpenseClaimService
        (
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache
        )
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<ApiResponse<string>> AddExpenseClaim(CreateExpenseClaimDTO dto)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = _mapper.Map<Fincore.Domain.Models.ExpenseClaim>(dto);
            entity.ApprovalStatus = "Pending";
            entity.CreatedAt = DateTime.Now;

            await _context.ExpenseClaims.AddAsync(entity);
            await _context.SaveChangesAsync();

            _cache.Remove("ExpenseClaimList");

            response.success = true;
            response.message = "Expense Claim Added Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<List<ExpenseClaimResponseDTO>>> GetExpenseClaims(int page, int pageSize)
        {
            ApiResponse<List<ExpenseClaimResponseDTO>> response =
                new ApiResponse<List<ExpenseClaimResponseDTO>>();

            string cacheKey = $"ExpenseClaimList_{page}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out List<ExpenseClaimResponseDTO> data))
            {
                var list = await _context.ExpenseClaims
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

                data = _mapper.Map<List<ExpenseClaimResponseDTO>>(list);

                _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
            }

            response.success = true;
            response.message = "Expense Claims Fetched Successfully";
            response.data = data;
            response.totalNumberRecord = data.Count;

            return response;
        }
        public async Task<ApiResponse<ExpenseClaimResponseDTO>> GetExpenseClaimById(int id)
        {
            ApiResponse<ExpenseClaimResponseDTO> response =
                new ApiResponse<ExpenseClaimResponseDTO>();

            string cacheKey = $"ExpenseClaim_{id}";

            if (!_cache.TryGetValue(cacheKey, out ExpenseClaimResponseDTO dto))
            {
                var entity = await _context.ExpenseClaims
                    .FirstOrDefaultAsync(x => x.ExpenseClaimId == id);

                if (entity == null)
                {
                    response.success = false;
                    response.message = "Expense Claim Not Found";
                    return response;
                }

                dto = _mapper.Map<ExpenseClaimResponseDTO>(entity);

                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
            }

            response.success = true;
            response.message = "Expense Claim Found Successfully";
            response.data = dto;

            return response;
        }
        public async Task<ApiResponse<string>> UpdateExpenseClaim(int id, UpdateExpenseClaimDTO dto)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.ExpenseClaims
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Expense Claim Not Found";

                return response;
            }

            _mapper.Map(dto, entity);

            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _cache.Remove("ExpenseClaimList");
            _cache.Remove($"ExpenseClaim_{id}");

            response.success = true;
            response.message = "Expense Claim Updated Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<string>> DeleteExpenseClaim(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.ExpenseClaims
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Expense Claim Not Found";

                return response;
            }

            _context.ExpenseClaims.Remove(entity);

            await _context.SaveChangesAsync();

            _cache.Remove("ExpenseClaimList");
            _cache.Remove($"ExpenseClaim_{id}");

            response.success = true;
            response.message = "Expense Claim Deleted Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<string>> ApproveExpenseClaim(int id, int approvedBy)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.ExpenseClaims
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Expense Claim Not Found";
                return response;
            }

            entity.ApprovalStatus = "Approved";
            entity.ApprovedBy = approvedBy;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _cache.Remove("ExpenseClaimList");
            _cache.Remove($"ExpenseClaim_{id}");

            response.success = true;
            response.message = "Expense Claim Approved Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<string>> RejectExpenseClaim(int id, int approvedBy)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.ExpenseClaims
                .FirstOrDefaultAsync(x => x.ExpenseClaimId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Expense Claim Not Found";
                return response;
            }

            entity.ApprovalStatus = "Rejected";
            entity.ApprovedBy = approvedBy;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _cache.Remove("ExpenseClaimList");
            _cache.Remove($"ExpenseClaim_{id}");

            response.success = true;
            response.message = "Expense Claim Rejected Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<ExpenseClaimSummaryDTO>> GetExpenseClaimSummary()
        {
            ApiResponse<ExpenseClaimSummaryDTO> response =
                new ApiResponse<ExpenseClaimSummaryDTO>();

            ExpenseClaimSummaryDTO summary = new ExpenseClaimSummaryDTO();

            summary.TotalClaims = await _context.ExpenseClaims.CountAsync();

            summary.ApprovedClaims = await _context.ExpenseClaims
                .CountAsync(x => x.ApprovalStatus == "Approved");

            summary.RejectedClaims = await _context.ExpenseClaims
                .CountAsync(x => x.ApprovalStatus == "Rejected");

            summary.PendingClaims = await _context.ExpenseClaims
                .CountAsync(x => x.ApprovalStatus == "Pending");

            summary.TotalExpenseAmount = await _context.ExpenseClaims
                .SumAsync(x => x.ExpenseAmount);

            response.success = true;
            response.message = "Expense Claim Summary Fetched Successfully";
            response.data = summary;

            return response;
        }
    }
}