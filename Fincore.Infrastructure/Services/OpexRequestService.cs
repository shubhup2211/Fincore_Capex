//using AutoMapper;
//using Fincore.Application.Common.Pagination;
//using Fincore.Application.DTO;
////using Fincore.Application.DTO.Opex;
////using Fincore.Application.DTO.Pagination;
//using Fincore.Application.DTOs.OpexRequest;
//using Fincore.Application.Interfaces.Opex;
//using Fincore.Domain.Models;
//using Fincore.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Memory;

//namespace Fincore.Infrastructure.Services.Opex
//{
//    public class OpexRequestService : IOpexRequestService
//    {
//        private readonly AppDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly IMemoryCache _cache;

//        public OpexRequestService(
//            AppDbContext context,
//            IMapper mapper,
//            IMemoryCache cache)
//        {
//            _context = context;
//            _mapper = mapper;
//            _cache = cache;
//        }

//        //public Task<ApiResponse<OpexRequestResponseDTO>> CreateAsync(CreateOpexRequestDTO dto)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<ApiResponse<string>> DeleteAsync(int id)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<ApiResponse<List<OpexRequestResponseDTO>>> GetAllAsync()
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<ApiResponse<OpexRequestResponseDTO>> GetByIdAsync(int id)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<ApiResponse<OpexRequestResponseDTO>> UpdateAsync(int id, UpdateOpexRequestDTO dto)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        // Methods will come here
//        public async Task<ApiResponse<OpexRequestResponseDTO>> CreateAsync(CreateOpexRequestDTO dto)
//        {
//            var response = new ApiResponse<OpexRequestResponseDTO>();

//            try
//            {
//                var entity = _mapper.Map<OpexRequest>(dto);

//                entity.CreatedAt = DateTime.UtcNow;
//                entity.ApprovalStatus = "Pending";

//                await _context.OpexRequests.AddAsync(entity);

//                await _context.SaveChangesAsync();

//                response.success = true;
//                response.message = "Opex Request created successfully.";
//                response.data = _mapper.Map<OpexRequestResponseDTO>(entity);

//                return response;
//            }
//            catch (Exception ex)
//            {
//                response.success = false;
//                response.message = ex.Message;

//                return response;
//            }
//        }


//        public async Task<ApiResponse<List<OpexRequestResponseDTO>>> GetAllAsync(PaginationRequest request)
//        {
//            var response = new ApiResponse<List<OpexRequestResponseDTO>>();

//            try
//            {
//                string cacheKey = $"OpexRequestList_{request.PageNumber}_{request.PageSize}";

//                // Check Cache
//                if (!_cache.TryGetValue(cacheKey, out List<OpexRequestResponseDTO> cachedData))
//                {
//                    // Total Records
//                    var totalRecords = await _context.OpexRequests.CountAsync();

//                    // Fetch Paginated Data
//                    var opexRequests = await _context.OpexRequests
//                        .OrderBy(x => x.OpexRequestId)
//                        .Skip((request.PageNumber - 1) * request.PageSize)
//                        .Take(request.PageSize)
//                        .ToListAsync();

//                    // Entity -> DTO
//                    cachedData = _mapper.Map<List<OpexRequestResponseDTO>>(opexRequests);

//                    // Store in Cache
//                    var cacheOptions = new MemoryCacheEntryOptions()
//                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

//                    _cache.Set(cacheKey, cachedData, cacheOptions);

//                    response.totalNumberRecord = totalRecords;

//                    response.metadata = new
//                    {
//                        request.PageNumber,
//                        request.PageSize,
//                        TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
//                        HasPreviousPage = request.PageNumber > 1,
//                        HasNextPage = request.PageNumber * request.PageSize < totalRecords
//                    };
//                }
//                else
//                {
//                    response.totalNumberRecord = cachedData.Count;
//                }

//                response.success = true;
//                response.message = "Opex Requests fetched successfully.";
//                response.data = cachedData;

//                return response;
//            }
//            catch (Exception ex)
//            {
//                response.success = false;
//                response.message = ex.Message;

//                return response;
//            }
//        }
//        public async Task<ApiResponse<OpexRequestResponseDTO>> GetByIdAsync(int id)
//        {
//            var response = new ApiResponse<OpexRequestResponseDTO>();

//            try
//            {
//                // Fetch record by Id
//                var opexRequest = await _context.OpexRequests
//                    .FirstOrDefaultAsync(x => x.OpexRequestId == id);

//                if (opexRequest == null)
//                {
//                    response.success = false;
//                    response.message = $"Opex Request with Id {id} not found.";

//                    return response;
//                }

//                // Entity -> DTO
//                var result = _mapper.Map<OpexRequestResponseDTO>(opexRequest);

//                response.success = true;
//                response.message = "Opex Request fetched successfully.";
//                response.data = result;

//                return response;
//            }
//            catch (Exception ex)
//            {
//                response.success = false;
//                response.message = ex.Message;

//                return response;
//            }
//        }
//        public async Task<ApiResponse<OpexRequestResponseDTO>> UpdateAsync(int id, UpdateOpexRequestDTO dto)
//        {
//            var response = new ApiResponse<OpexRequestResponseDTO>();

//            try
//            {
//                // Check if record exists
//                var entity = await _context.OpexRequests
//                    .FirstOrDefaultAsync(x => x.OpexRequestId == id);

//                if (entity == null)
//                {
//                    response.success = false;
//                    response.message = $"Opex Request with Id {id} not found.";

//                    return response;
//                }

//                // Update Entity using AutoMapper
//                _mapper.Map(dto, entity);

//                // Update Audit Field
//                entity.ModifiedAt = DateTime.UtcNow;

//                // Save Changes
//                await _context.SaveChangesAsync();

//                // Remove cache
//                _cache.Remove("OpexRequestList");

//                // Convert Entity to DTO
//                var result = _mapper.Map<OpexRequestResponseDTO>(entity);

//                response.success = true;
//                response.message = "Opex Request updated successfully.";
//                response.data = result;

//                return response;
//            }
//            catch (Exception ex)
//            {
//                response.success = false;
//                response.message = ex.Message;

//                return response;
//            }
//        }
//        public async Task<ApiResponse<string>> DeleteAsync(int id)
//        {
//            var response = new ApiResponse<string>();

//            try
//            {
//                // Check if record exists
//                var entity = await _context.OpexRequests
//                    .FirstOrDefaultAsync(x => x.OpexRequestId == id);

//                if (entity == null)
//                {
//                    response.success = false;
//                    response.message = $"Opex Request with Id {id} not found.";

//                    return response;
//                }

//                // Delete record
//                _context.OpexRequests.Remove(entity);

//                // Save changes
//                await _context.SaveChangesAsync();

//                // Clear Cache
//                _cache.Remove("OpexRequestList");

//                response.success = true;
//                response.message = "Opex Request deleted successfully.";
//                response.data = "Record Deleted Successfully.";

//                return response;
//            }
//            catch (Exception ex)
//            {
//                //response.success = false;
//                //response.message = ex.Message;

//                return response;
//            }
//        }
//    }
//}