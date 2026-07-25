using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTOs.WorkOrder;
using Fincore.Application.Interfaces.WorkOrder;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.Infrastructure.Services.WorkOrder
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public WorkOrderService(
            AppDbContext context,
            IMapper mapper,
            IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse<string>> AddWorkOrder(CreateWorkOrderDTO dto)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var entity = _mapper.Map<Fincore.Domain.Models.WorkOrder>(dto);


            entity.CreatedDate = DateTime.Now;

            await _context.WorkOrders.AddAsync(entity);
            await _context.SaveChangesAsync();

            _cache.Remove("WorkOrderList");

            response.success = true;
            response.message = "Work Order Added Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<List<WorkOrderResponseDTO>>> GetWorkOrders(int page, int pageSize)
        {
            ApiResponse<List<WorkOrderResponseDTO>> response =
                new ApiResponse<List<WorkOrderResponseDTO>>();

            string cacheKey = $"WorkOrderList_{page}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out List<WorkOrderResponseDTO> data))
            {
                var list = await _context.WorkOrders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                data = _mapper.Map<List<WorkOrderResponseDTO>>(list);

                _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
            }

            response.success = true;
            response.message = "Work Orders Fetched Successfully";
            response.data = data;
            response.totalNumberRecord = data.Count;

            return response;
        }
        public async Task<ApiResponse<WorkOrderResponseDTO>> GetWorkOrderById(int id)
        {
            ApiResponse<WorkOrderResponseDTO> response =
                new ApiResponse<WorkOrderResponseDTO>();

            string cacheKey = $"WorkOrder_{id}";

            if (!_cache.TryGetValue(cacheKey, out WorkOrderResponseDTO dto))
            {
                var entity = await _context.WorkOrders
                    .FirstOrDefaultAsync(x => x.WorkOrderId == id);

                if (entity == null)
                {
                    response.success = false;
                    response.message = "Work Order Not Found";
                    return response;
                }

                dto = _mapper.Map<WorkOrderResponseDTO>(entity);

                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
            }

            response.success = true;
            response.message = "Work Order Found Successfully";
            response.data = dto;

            return response;
        }
        public async Task<ApiResponse<string>> UpdateWorkOrder(int id, UpdateWorkOrderDTO dto)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Work Order Not Found";
                return response;
            }

            _mapper.Map(dto, entity);

            await _context.SaveChangesAsync();

            _cache.Remove("WorkOrderList");
            _cache.Remove($"WorkOrder_{id}");

            response.success = true;
            response.message = "Work Order Updated Successfully";
            response.data = "Success";

            return response;
        }

        public async Task<ApiResponse<string>> DeleteWorkOrder(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Work Order Not Found";
                return response;
            }

            _context.WorkOrders.Remove(entity);

            await _context.SaveChangesAsync();

            _cache.Remove("WorkOrderList");
            _cache.Remove($"WorkOrder_{id}");

            response.success = true;
            response.message = "Work Order Deleted Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<string>> ApproveWorkOrder(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Work Order Not Found";
                return response;
            }

            entity.Status = "Approved";

            await _context.SaveChangesAsync();

            _cache.Remove("WorkOrderList");
            _cache.Remove($"WorkOrder_{id}");

            response.success = true;
            response.message = "Work Order Approved Successfully";
            response.data = "Success";

            return response;
        }

        public async Task<ApiResponse<string>> RejectWorkOrder(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var entity = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == id);

            if (entity == null)
            {
                response.success = false;
                response.message = "Work Order Not Found";
                return response;
            }

            entity.Status = "Rejected";

            await _context.SaveChangesAsync();

            _cache.Remove("WorkOrderList");
            _cache.Remove($"WorkOrder_{id}");

            response.success = true;
            response.message = "Work Order Rejected Successfully";
            response.data = "Success";

            return response;
        }
        public async Task<ApiResponse<WorkOrderSummaryDTO>> GetWorkOrderSummary()
        {
            ApiResponse<WorkOrderSummaryDTO> response =
                new ApiResponse<WorkOrderSummaryDTO>();

            WorkOrderSummaryDTO summary = new WorkOrderSummaryDTO();

            summary.TotalWorkOrders =
                await _context.WorkOrders.CountAsync();

            summary.PendingWorkOrders =
                await _context.WorkOrders
                    .CountAsync(x => x.Status == "Pending");

            summary.CompletedWorkOrders =
                await _context.WorkOrders
                    .CountAsync(x => x.Status == "Completed");

            summary.TotalNetAmount =
                await _context.WorkOrders
                    .SumAsync(x => x.NetAmount);

            response.success = true;
            response.message = "Work Order Summary Fetched Successfully";
            response.data = summary;

            return response;
        }
    }
}