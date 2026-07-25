using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTOs.OpexRequest;
using Fincore.Application.Interfaces.Opex;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.Opex
{
    public class OpexRequestService : IOpexRequestService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OpexRequestService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create
        public async Task AddOpexRequest(CreateOpexRequestDTO dto)
        {
            var entity = _mapper.Map<OpexRequest>(dto);

            entity.ApprovalStatus = "Pending";
            entity.CreatedAt = DateTime.UtcNow;

            await _context.OpexRequests.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Get All
        public async Task<List<OpexRequestResponseDTO>> GetOpexRequests(int page, int pageSize)
        {
            var opexList = await _context.OpexRequests
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return _mapper.Map<List<OpexRequestResponseDTO>>(opexList);
        }

        // Get By Id
        public async Task<OpexRequestResponseDTO?> GetOpexRequestById(int id)
        {
            var entity = await _context.OpexRequests
                                .FirstOrDefaultAsync(x => x.OpexRequestId == id);

            if (entity == null)
            {
                return null;
            }

            return _mapper.Map<OpexRequestResponseDTO>(entity);
        }

        // Update
        public async Task UpdateOpexRequest(int id, UpdateOpexRequestDTO dto)
        {
            var entity = await _context.OpexRequests
                                .FirstOrDefaultAsync(x => x.OpexRequestId == id);

            if (entity == null)
            {
                return;
            }

            _mapper.Map(dto, entity);

            entity.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteOpexRequest(int id)
        {
            var entity = await _context.OpexRequests
                                .FirstOrDefaultAsync(x => x.OpexRequestId == id);

            if (entity == null)
            {
                return;
            }

            _context.OpexRequests.Remove(entity);

            await _context.SaveChangesAsync();
        }

      
        // apprve api 
        public async Task<string> ApproveOpexRequest(int id, int approvedBy)
        {
            var opex = await _context.OpexRequests
                .FirstOrDefaultAsync(x => x.OpexRequestId == id);

            if (opex == null)
            {
                return "Opex Request Not Found";
            }

            opex.ApprovalStatus = "Approved";
            opex.ApprovedBy = approvedBy;
            opex.ApprovedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return "Opex Request Approved Successfully";
        }

        public async Task<string> RejectOpexRequest(int id, int approvedBy)
        {
            var opex = await _context.OpexRequests
                .FirstOrDefaultAsync(x => x.OpexRequestId == id);

            if (opex == null)
            {
                return "Opex Request Not Found";
            }

            opex.ApprovalStatus = "Rejected";
            opex.ApprovedBy = approvedBy;
            opex.ApprovedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return "Opex Request Rejected Successfully";
        }

        public async Task<OpexSummaryDTO> GetOpexSummary()
        {
            OpexSummaryDTO summary = new OpexSummaryDTO();

            summary.TotalRequest =
                await _context.OpexRequests.CountAsync();

            summary.ApprovedRequest =
                await _context.OpexRequests
                    .CountAsync(x => x.ApprovalStatus == "Approved");

            summary.RejectedRequest =
                await _context.OpexRequests
                    .CountAsync(x => x.ApprovalStatus == "Rejected");

            summary.PendingRequest =
                await _context.OpexRequests
                    .CountAsync(x => x.ApprovalStatus == "Pending");

            summary.TotalAmount =
                await _context.OpexRequests
                    .SumAsync(x => x.Amount);

            return summary;
        }
    }
}