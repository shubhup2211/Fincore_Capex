using Fincore.Application.DTO;
using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces;
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



        // Create company
        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyDto dto)
        {
            var result = await companyService.CreateCompanyAsync(dto);

            if (!result.success)
            {
                if (result.Error?.code == "404")
                    return NotFound(result);

                if (result.Error?.code == "409")
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        // Get all companies
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies(int page = 1, int limit = 10)
        {
            var result = await companyService.GetAllCompaniesAsync(page, limit);

            if (!result.success)
            {
                if (result.Error?.code == "404")
                    return NotFound(result);

                if (result.Error?.code == "409")
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        // Get company by id
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            var result = await companyService.GetCompanyByIdAsync(companyId);

            if (!result.success)
            {
                if (result.Error?.code == "404")
                    return NotFound(result);

                if (result.Error?.code == "409")
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        // Update company
        [HttpPut]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyDto dto)
        {
            var result = await companyService.UpdateCompanyAsync(dto);

            if (!result.success)
            {
                if (result.Error?.code == "404")
                    return NotFound(result);

                if (result.Error?.code == "409")
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        // Delete company
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await companyService.DeleteCompanyAsync(companyId);

            if (!result.success)
            {
                if (result.Error?.code == "404")
                    return NotFound(result);

                if (result.Error?.code == "409")
                    return Conflict(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}