using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.MasterTable
{
    [ApiController]
    [Route("api/v1/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService companyService;

        // Constructor
        public CompaniesController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        // Return response
        private IActionResult GetResponse<T>(ApiResponse<T> result)
        {
            if (result.success)
            {
                return Ok(result);
            }

            return result.Error?.code switch
            {
                "404" => NotFound(result),
                "409" => Conflict(result),
                _ => BadRequest(result)
            };
        }

        // Create company
        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyDto dto)
        {
            var result = await companyService.CreateCompanyAsync(dto);

            return GetResponse(result);
        }

        // Get all companies
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies(int page = 1, int limit = 10)
        {
            var result = await companyService.GetAllCompaniesAsync(page, limit);

            return GetResponse(result);
        }

        // Get company by id
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            var result = await companyService.GetCompanyByIdAsync(companyId);

            return GetResponse(result);
        }

        // Update company
        [HttpPut]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyDto dto)
        {
            var result = await companyService.UpdateCompanyAsync(dto);

            return GetResponse(result);
        }

        // Delete company
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await companyService.DeleteCompanyAsync(companyId);

            return GetResponse(result);
        }
    }
}