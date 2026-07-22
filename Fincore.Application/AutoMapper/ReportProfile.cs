using AutoMapper;
using Fincore.Application.DTO.Reports;
using Fincore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.AutoMapper
{
     public  class ReportProfile : Profile
    {
        public ReportProfile()
        {
            // Revenue
            CreateMap<RevenueEntry, RevenueReportDTO>()
                .ForMember(dest => dest.CustomerCode, opt => opt.MapFrom(src => src.Customer.CustomerCode))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.AccountCode, opt => opt.MapFrom(src => src.AccountMaster.AccountCode))
                .ForMember(dest => dest.AccountName,  opt => opt.MapFrom(src => src.AccountMaster.AccountName));

            // Expense
            CreateMap<ExpenseClaim, ExpenseReportDTO>()
            .ForMember(dest => dest.OpexTitle,opt => opt.MapFrom(src => src.OpexRequest.Title))
            .ForMember(dest => dest.ClaimedBy, opt => opt.MapFrom(src => src.ClaimByUser.FullName));

            CreateMap<Payment, VendorSpendDTO>()
                .ForMember(dest => dest.VendorCode, opt => opt.MapFrom(src => src.Vendor.VendorCode))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Vendor.Company.CompanyName));

            //Capex
            CreateMap<CapexRequest, CapexReportDTO>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName));

            // Opex
            CreateMap<OpexRequest, OpexReportDTO>();

            // budget variamce
            CreateMap<BudgetLine, BudgetVarianceReportDTO>()
                .ForMember(dest => dest.BudgetCode,opt => opt.MapFrom(src => src.Budget.BudgetCode))
                .ForMember(dest => dest.BudgetName, opt => opt.MapFrom(src => src.Budget.BudgetName))
                .ForMember(dest => dest.FinancialYear,opt => opt.MapFrom(src => src.Budget.FinancialYear))
                .ForMember(dest => dest.BudgetAmount,opt => opt.MapFrom(src => src.Budget.BudgetAmount))
                .ForMember(dest => dest.CategoryName,opt => opt.MapFrom(src => src.BudgetCategory.CategoryName))
                .ForMember(dest => dest.DepartmentName,opt => opt.MapFrom(src => src.BudgetCategory.Department.DepartmentName))
                .ForMember(dest => dest.UtilizedAmount,opt => opt.MapFrom(src => src.UtilizedAmount ?? 0))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.AllocatedAmount - (src.UtilizedAmount ?? 0)));
        }
    }
}
