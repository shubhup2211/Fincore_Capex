using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fincore.Application.Interfaces.IMasterTable
{
    public interface ICompanyService
    {
        // Create company
        Task<ApiResponse<CompanyDto>> CreateCompanyAsync(CreateCompanyDto dto);

        // Get all companies
        Task<ApiResponse<List<CompanyDto>>> GetAllCompaniesAsync(int page, int limit);

        // Get company by id
        Task<ApiResponse<CompanyDto>> GetCompanyByIdAsync(int companyId);

        // Update company
        Task<ApiResponse<CompanyDto>> UpdateCompanyAsync(UpdateCompanyDto dto);

        // Delete company
        Task<ApiResponse<string>> DeleteCompanyAsync(int companyId);
    }
}
