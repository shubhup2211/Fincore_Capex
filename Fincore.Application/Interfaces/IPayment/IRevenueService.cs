using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment.RevenueEntry.Requests;
using Fincore.Application.DTO.Payment.RevenueEntry.Responses;
using System;

namespace Fincore.Application.Interfaces.IPayment
{
    public interface IRevenueService
    {
        // CRUD

        Task<ApiResponse<RevenueEntryResponseDto>> CreateAsync(CreateRevenueEntryRequestDto request);

        Task<ApiResponse<List<RevenueEntryResponseDto>>> GetAllAsync(int page, int pageSize);

        Task<ApiResponse<RevenueEntryResponseDto>> GetByIdAsync(int revenueEntryId);

        Task<ApiResponse<RevenueEntryResponseDto>> UpdateAsync(
                                                                int revenueEntryId,
                                                                UpdateRevenueEntryRequestDto request);

        Task<ApiResponse<string>> DeleteAsync(int revenueEntryId);

        // Filters

        Task<ApiResponse<List<RevenueEntryResponseDto>>> GetRevenueByStatusAsync(
            string status,
            int page,
            int pageSize);

        Task<ApiResponse<List<RevenueEntryResponseDto>>> GetRevenueByTypeAsync(
            string revenueType,
            int page,
            int pageSize);

        // Reports

        Task<ApiResponse<List<MonthlyRevenueDto>>> GetMonthlyRevenueAsync();

        Task<ApiResponse<RevenueSummaryDto>> GetRevenueSummaryAsync();


        //Task<ApiResponse<RevenueEntryResponseDto>> ApproveAsync(int revenueEntryId);

        //Task<ApiResponse<RevenueEntryResponseDto>> RejectAsync(int revenueEntryId, string? remarks = null);

        Task<ApiResponse<RevenueEntryResponseDto>> MarkAsInvoicedAsync(int revenueEntryId);
        Task<ApiResponse<RevenueEntryResponseDto>> MarkAsReceivedAsync(int revenueEntryId);
    }
}
