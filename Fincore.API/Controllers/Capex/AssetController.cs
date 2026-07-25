using Fincore.Application.DTO.Capex;
using Fincore.Application.Interfaces.ICapex;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Fincore.API.Controllers.Capex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public class AssetController : ControllerBase
    {

        private readonly IAssetService service;


        public AssetController(IAssetService service)
        {
            this.service = service;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsset(AssetDTO dto)
        {
            return Ok(await service.AddAsset(dto));
        }



        [HttpGet]
        public async Task<IActionResult> GetAllAssets(
            int page = 1,
            int pageSize = 10)
        {
            return Ok(await service.GetAllAssets(page, pageSize));
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsset(int id)
        {
            return Ok(await service.GetAsset(id));
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(
            int id,
            AssetDTO dto)
        {
            return Ok(await service.UpdateAsset(id, dto));
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            return Ok(await service.DeleteAsset(id));
        }


        [HttpPut("{id}/assign")]
        public async Task<IActionResult> Assign(
int id, int userId)
        {
            return Ok(
            await service.AssignAsset(id, userId));
        }



        [HttpPut("{id}/transfer")]
        public async Task<IActionResult> Transfer(
        int id, int departmentId)
        {
            return Ok(
            await service.TransferAsset(id, departmentId));
        }



        [HttpPut("{id}/dispose")]
        public async Task<IActionResult> Dispose(int id)
        {
            return Ok(
            await service.DisposeAsset(id));
        }



        [HttpPut("{id}/repair")]
        public async Task<IActionResult> Repair(int id)
        {
            return Ok(
            await service.RepairAsset(id));
        }



        [HttpPut("{id}/return")]
        public async Task<IActionResult> Return(int id)
        {
            return Ok(
            await service.ReturnAsset(id));
        }
    }
}