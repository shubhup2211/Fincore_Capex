using Fincore.Application.DTO.MasterTable;
using Fincore.Application.Interfaces.IMasterTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.MasterTable
{
    [Route("api/v2/[Action]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class MasterTypeController : ControllerBase
    {
        IMasterType repo;

        public MasterTypeController(IMasterType repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMasterType(int page = 1, int pageSize = 10)
        {
            var data = await repo.GetAllMasterType(page, pageSize);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdMasterType(int id)
        {
            var data = await repo.GetByIdMasterType(id);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddMasterType(CreateMasterTypeDto dto)
        {
            var data = await repo.AddMasterType(dto);
            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMasterType(int id, UpdateMasterTypeDto dto)
        {
            var data = await repo.UpdateMasterType(id, dto);
            return Ok(data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMasterType(int id)
        {
            var data = await repo.DeleteMasterType(id);
            return Ok(data);
        }
    }
}