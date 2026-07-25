using AutoMapper;
using Fincore.Application.DTOs.Budget;
using Fincore.Application.DTOs.BudgetCategory;
using Fincore.Application.DTOs.BudgetLine;
using Fincore.Application.DTOs.ExpenseClaim;
using Fincore.Application.DTOs.OpexRequest;
using Fincore.Application.DTOs.WorkOrder;
using Fincore.Domain.Models;
namespace Fincore.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Opex Request
            CreateMap<OpexRequest, CreateOpexRequestDTO>().ReverseMap();
            CreateMap<OpexRequest, UpdateOpexRequestDTO>().ReverseMap();
            CreateMap<OpexRequest, OpexRequestResponseDTO>();

            // Expense Claim
            CreateMap<ExpenseClaim, CreateExpenseClaimDTO>().ReverseMap();
            CreateMap<ExpenseClaim, UpdateExpenseClaimDTO>().ReverseMap();
            CreateMap<ExpenseClaim, ExpenseClaimResponseDTO>();

            // Work Order
            CreateMap<WorkOrder, CreateWorkOrderDTO>().ReverseMap();

            CreateMap<WorkOrder, UpdateWorkOrderDTO>().ReverseMap();

            CreateMap<WorkOrder, WorkOrderResponseDTO>();

            CreateMap<BudgetCategory, CreateBudgetCategoryDTO>().ReverseMap();

            CreateMap<BudgetCategory, UpdateBudgetCategoryDTO>().ReverseMap();

            CreateMap<BudgetCategory, BudgetCategoryResponseDTO>();

            CreateMap<Budget, CreateBudgetDTO>().ReverseMap();

            CreateMap<Budget, UpdateBudgetDTO>().ReverseMap();

            CreateMap<Budget, BudgetResponseDTO>();

            CreateMap<BudgetLine, CreateBudgetLineDTO>().ReverseMap();

            CreateMap<BudgetLine, UpdateBudgetLineDTO>().ReverseMap();

            CreateMap<BudgetLine, BudgetLineResponseDTO>();
        }
    }
}