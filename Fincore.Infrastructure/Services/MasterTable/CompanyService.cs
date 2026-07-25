using AutoMapper;
using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Fincore.Domain.Models;
using Fincore.Infrastructure.CommonHelper;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services.MasterTable
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public CompanyService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        private IQueryable<Company> GetCompanyQuery()
        {
            return db.Companies
                .Where(x => x.IsActive == 1)
                .Include(x => x.Country)
                .Include(x => x.MasterType);
        }

        private async Task<string> GenerateCompanyCodeAsync()
        {
            Company lastCompany = await db.Companies
                .Where(x => x.CompanyCode.StartsWith("COM"))
                .OrderByDescending(x => x.CompanyId)
                .FirstOrDefaultAsync();

            if (lastCompany == null)
            {
                return "COM0001";
            }

            string numberPart = lastCompany.CompanyCode.Substring(3);

            int companyNumber;

            if (!int.TryParse(numberPart, out companyNumber))
            {
                companyNumber = 0;
            }

            companyNumber++;

            return $"COM{companyNumber:D4}";
        }

        public async Task<ApiResponse<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto)
        {
            Company existingCompany = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyName.ToLower() == dto.CompanyName.ToLower());

            if (existingCompany != null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company already exists.",
                    "409",
                    "Company name already exists.");
            }

            string companyCode = await GenerateCompanyCodeAsync();

            Company existingCode = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyCode == companyCode);

            if (existingCode != null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company code already exists.",
                    "409",
                    "Company code already exists.");
            }

            Company newCompany = mapper.Map<Company>(dto);

            newCompany.CompanyCode = companyCode;

            newCompany.IsActive = 1;

            newCompany.CreatedAt = DateTime.Now;
            newCompany.CreatedBy = 1;

            await db.Companies.AddAsync(newCompany);
            await db.SaveChangesAsync();

            await db.Entry(newCompany)
                .Reference(x => x.Country)
                .LoadAsync();

            if (newCompany.MasterTypeId != null)
            {
                await db.Entry(newCompany)
                    .Reference(x => x.MasterType)
                    .LoadAsync();
            }

            CompanyDto companyDto = mapper.Map<CompanyDto>(newCompany);

            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company created successfully.");
        }

        public async Task<ApiResponse<List<CompanyDto>>> GetAllCompaniesAsync(int page, int limit)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (limit <= 0)
            {
                limit = 10;
            }

            int totalRecords = await db.Companies
                .Where(x => x.IsActive == 1)
                .CountAsync();

            List<Company> companies = await GetCompanyQuery()
                .OrderBy(x => x.CompanyId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            List<CompanyDto> companyList = mapper.Map<List<CompanyDto>>(companies);

            var metadata = new
            {
                CurrentPage = page,
                PageSize = limit,
                TotalPages = (int)Math.Ceiling((double)totalRecords / limit)
            };

            return ApiResponseHelper.SuccessRes(
                companyList,
                "Companies fetched successfully.",
                totalRecords,
                metadata);
        }

        public async Task<ApiResponse<CompanyDto>> GetCompanyByIdAsync(int companyId)
        {
            Company company = await GetCompanyQuery()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId);

            if (company == null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            CompanyDto companyDto = mapper.Map<CompanyDto>(company);

            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company fetched successfully.");
        }

        public async Task<ApiResponse<CompanyDto>> UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            Company company = await GetCompanyQuery()
                .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId);

            if (company == null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            Company existingCompany = await db.Companies
                .FirstOrDefaultAsync(x =>
                    x.CompanyName.ToLower() == dto.CompanyName.ToLower() &&
                    x.CompanyId != dto.CompanyId);

            if (existingCompany != null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company already exists.",
                    "409",
                    "Company name already exists.");
            }

            company.CompanyName = dto.CompanyName;
            company.CountryId = dto.CountryId;
            company.ContactNumber = dto.ContactNumber;
            company.ContactEmail = dto.ContactEmail;
            company.GSTIN = dto.GSTIN;
            company.CIN = dto.CIN;
            company.PAN = dto.PAN;
            company.TAN = dto.TAN;
            company.Address = dto.Address;
            company.MasterTypeId = dto.MasterTypeId;

            company.ModifiedAt = DateTime.Now;
            company.ModifiedBy = 1;

            await db.SaveChangesAsync();

            await db.Entry(company)
                .Reference(x => x.Country)
                .LoadAsync();

            if (company.MasterTypeId != null)
            {
                await db.Entry(company)
                    .Reference(x => x.MasterType)
                    .LoadAsync();
            }

            CompanyDto companyDto = mapper.Map<CompanyDto>(company);

            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteCompanyAsync(int companyId)
        {
            Company company = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyId == companyId);

            if (company == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            
            if (company.IsActive == 0)
            {
                return ApiResponseHelper.Failure<string>(
                    "Company already deleted.",
                    "409",
                    "Company is already inactive.");
            }


            company.IsActive = 0;
            company.ModifiedAt = DateTime.Now;
            company.ModifiedBy = 1;

         
            await db.SaveChangesAsync();

        
            return ApiResponseHelper.SuccessRes(
                "Deleted",
                "Company deleted successfully.");
        }
    }
}