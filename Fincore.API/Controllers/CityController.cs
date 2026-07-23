using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _cityService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var city = await _cityService.GetByIdAsync(id);

            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CityRequestDto dto)
        {
            var city = new City
            {
                CityName = dto.CityName,
                StateId = dto.StateId
            };

            var result = await _cityService.CreateAsync(city);

            return CreatedAtAction(nameof(GetById), new { id = result.CityId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CityRequestDto dto)
        {
            var city = new City
            {
                CityName = dto.CityName,
                StateId = dto.StateId
            };

            var result = await _cityService.UpdateAsync(id, city);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _cityService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok("City deleted successfully.");
        }
    }
}