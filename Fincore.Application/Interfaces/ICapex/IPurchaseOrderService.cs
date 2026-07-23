using Fincore.Application.DTO;
using Fincore.Application.DTO.Capex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.ICapex
{
    public interface IPurchaseOrderService
    {
        Task<ApiResponse<PurchaseOrderDTO>> AddPurchaseOrder(PurchaseOrderDTO dto);

        Task<ApiResponse<PurchaseOrderDTO>> GetPurchaseOrder(int id);

        Task<ApiResponse<List<PurchaseOrderDTO>>> GetAllPurchaseOrder(int page, int pageSize);

        Task<ApiResponse<PurchaseOrderDTO>> UpdatePurchaseOrder(int id, PurchaseOrderDTO dto);

        Task<ApiResponse<PurchaseOrderDTO>> DeletePurchaseOrder(int id);
    }
}
