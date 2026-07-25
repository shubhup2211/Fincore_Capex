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
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedPolicy")]
    public partial class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CountryController(
            ICountryService countryService,
            IMapper mapper,
            IMemoryCache cache)
        {
            _countryService = countryService;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {
            string cacheKey = $"Country_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out PagedResponse<CountryResponseDto>? response))
            {
                var countries = await _countryService.GetAllAsync(pageNumber, pageSize);

                response = new PagedResponse<CountryResponseDto>
                {
                    PageNumber = countries.PageNumber,
                    PageSize = countries.PageSize,
                    TotalRecords = countries.TotalRecords,
                    TotalPages = countries.TotalPages,
                    Data = _mapper.Map<List<CountryResponseDto>>(countries.Data)
                };

                _cache.Set(
                    cacheKey,
                    response,
                    TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string cacheKey = $"Country_{id}";

            if (!_cache.TryGetValue(cacheKey, out CountryResponseDto? response))
            {
                var country = await _countryService.GetByIdAsync(id);

                if (country == null)
                    return NotFound("Country not found.");

                response = _mapper.Map<CountryResponseDto>(country);

                _cache.Set(
                    cacheKey,
                    response,
                    TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CountryRequestDto dto)
        {
            var country = _mapper.Map<Country>(dto);

            var createdCountry = await _countryService.CreateAsync(country);

            // Clear paginated cache
            _cache.Remove("Country_1_10");

            var response = _mapper.Map<CountryResponseDto>(createdCountry);

            return CreatedAtAction(nameof(GetById),
                new { id = response.CountryId },
                response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CountryRequestDto dto)
        {
            var country = _mapper.Map<Country>(dto);

            var updatedCountry = await _countryService.UpdateAsync(id, country);

            if (updatedCountry == null)
                return NotFound("Country not found.");

            // Clear cache
            _cache.Remove($"Country_{id}");
            _cache.Remove("Country_1_10");

            var response = _mapper.Map<CountryResponseDto>(updatedCountry);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _countryService.DeleteAsync(id);

                if (!result)
                    return NotFound("Country not found.");

                // Clear cache
                _cache.Remove($"Country_{id}");
                _cache.Remove("Country_1_10");

                return Ok("Country deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}