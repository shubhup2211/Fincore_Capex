using AutoMapper;
using Fincore.Application.CommonHelper;
using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("FixedPolicy")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CityController(
            ICityService cityService,
            IMapper mapper,
            IMemoryCache cache)
        {
            _cityService = cityService;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {
            string cacheKey = $"City_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out PagedResponse<CityResponseDto>? response))
            {
                var result = await _cityService.GetAllAsync(pageNumber, pageSize);

                response = new PagedResponse<CityResponseDto>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalRecords = result.TotalRecords,
                    TotalPages = result.TotalPages,
                    Data = _mapper.Map<List<CityResponseDto>>(result.Data)
                };

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string cacheKey = $"City_{id}";

            if (!_cache.TryGetValue(cacheKey, out CityResponseDto? response))
            {
                var city = await _cityService.GetByIdAsync(id);

                if (city == null)
                    return NotFound("City not found.");

                response = _mapper.Map<CityResponseDto>(city);

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CityRequestDto dto)
        {
            var city = _mapper.Map<City>(dto);

            var createdCity = await _cityService.CreateAsync(city);

            _cache.Remove("City_1_10");

            var response = _mapper.Map<CityResponseDto>(createdCity);

            return CreatedAtAction(nameof(GetById),
                new { id = response.CityId },
                response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CityRequestDto dto)
        {
            var city = _mapper.Map<City>(dto);

            var updatedCity = await _cityService.UpdateAsync(id, city);

            if (updatedCity == null)
                return NotFound("City not found.");

            _cache.Remove($"City_{id}");
            _cache.Remove("City_1_10");

            var response = _mapper.Map<CityResponseDto>(updatedCity);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cityService.DeleteAsync(id);

            if (!result)
                return NotFound("City not found.");

            _cache.Remove($"City_{id}");
            _cache.Remove("City_1_10");

            return Ok("City deleted successfully.");
        }
    }
}