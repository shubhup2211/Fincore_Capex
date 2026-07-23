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

        // Get active companies
        private IQueryable<Company> GetCompanyQuery()
        {
            return db.Companies
                .Where(x => x.IsActive == 1)
                .Include(x => x.Country)
                .Include(x => x.MasterType);
        }

        // Generate company code
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

        // Create company
        public async Task<ApiResponse<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto)
        {
            // Check duplicate company name
            Company existingCompany = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyName.ToLower() == dto.CompanyName.ToLower());

            if (existingCompany != null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company already exists.",
                    "409",
                    "Company name already exists.");
            }

            // Generate company code
            string companyCode = await GenerateCompanyCodeAsync();

            // Check duplicate company code
            Company existingCode = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyCode == companyCode);

            if (existingCode != null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company code already exists.",
                    "409",
                    "Company code already exists.");
            }

            // Convert DTO to entity
            Company newCompany = mapper.Map<Company>(dto);

            // Set company code
            newCompany.CompanyCode = companyCode;

            // Set active status
            newCompany.IsActive = 1;

            // Set audit fields
            newCompany.CreatedAt = DateTime.Now;
            newCompany.CreatedBy = 1;

            // Save company
            await db.Companies.AddAsync(newCompany);
            await db.SaveChangesAsync();

            // Load country
            await db.Entry(newCompany)
                .Reference(x => x.Country)
                .LoadAsync();

            // Load master type
            if (newCompany.MasterTypeId != null)
            {
                await db.Entry(newCompany)
                    .Reference(x => x.MasterType)
                    .LoadAsync();
            }

            // Convert entity to DTO
            CompanyDto companyDto = mapper.Map<CompanyDto>(newCompany);

            // Return response
            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company created successfully.");
        }

        // Get all companies
        public async Task<ApiResponse<List<CompanyDto>>> GetAllCompaniesAsync(int page, int limit)
        {
            // Check page
            if (page <= 0)
            {
                page = 1;
            }

            // Check limit
            if (limit <= 0)
            {
                limit = 10;
            }

            // Get total records
            int totalRecords = await db.Companies
                .Where(x => x.IsActive == 1)
                .CountAsync();

            // Get companies
            List<Company> companies = await GetCompanyQuery()
                .OrderBy(x => x.CompanyId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            // Convert entity to DTO
            List<CompanyDto> companyList = mapper.Map<List<CompanyDto>>(companies);

            // Pagination details
            var metadata = new
            {
                CurrentPage = page,
                PageSize = limit,
                TotalPages = (int)Math.Ceiling((double)totalRecords / limit)
            };

            // Return response
            return ApiResponseHelper.SuccessRes(
                companyList,
                "Companies fetched successfully.",
                totalRecords,
                metadata);
        }

        // Get company by id
        public async Task<ApiResponse<CompanyDto>> GetCompanyByIdAsync(int companyId)
        {
            // Get company
            Company company = await GetCompanyQuery()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId);

            // Check company
            if (company == null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            // Convert entity to DTO
            CompanyDto companyDto = mapper.Map<CompanyDto>(company);

            // Return response
            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company fetched successfully.");
        }

        // Update company
        public async Task<ApiResponse<CompanyDto>> UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            // Get company
            Company company = await GetCompanyQuery()
                .FirstOrDefaultAsync(x => x.CompanyId == dto.CompanyId);

            // Check company
            if (company == null)
            {
                return ApiResponseHelper.Failure<CompanyDto>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            // Check duplicate company name
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

            // Update details
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

            // Update audit fields
            company.ModifiedAt = DateTime.Now;
            company.ModifiedBy = 1;

            // Save changes
            await db.SaveChangesAsync();

            // Load country
            await db.Entry(company)
                .Reference(x => x.Country)
                .LoadAsync();

            // Load master type
            if (company.MasterTypeId != null)
            {
                await db.Entry(company)
                    .Reference(x => x.MasterType)
                    .LoadAsync();
            }

            // Convert entity to DTO
            CompanyDto companyDto = mapper.Map<CompanyDto>(company);

            // Return response
            return ApiResponseHelper.SuccessRes(
                companyDto,
                "Company updated successfully.");
        }

        // Delete company
        public async Task<ApiResponse<string>> DeleteCompanyAsync(int companyId)
        {
            // Get company
            Company company = await db.Companies
                .FirstOrDefaultAsync(x => x.CompanyId == companyId);

            // Check company
            if (company == null)
            {
                return ApiResponseHelper.Failure<string>(
                    "Company not found.",
                    "404",
                    "Invalid Company Id.");
            }

            // Check already deleted
            if (company.IsActive == 0)
            {
                return ApiResponseHelper.Failure<string>(
                    "Company already deleted.",
                    "409",
                    "Company is already inactive.");
            }

            // Soft delete
            company.IsActive = 0;
            company.ModifiedAt = DateTime.Now;
            company.ModifiedBy = 1;

            // Save changes
            await db.SaveChangesAsync();

            // Return response
            return ApiResponseHelper.SuccessRes(
                "Deleted",
                "Company deleted successfully.");
        }
    }
}