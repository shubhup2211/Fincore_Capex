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
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CurrencyController(
            ICurrencyService currencyService,
            IMapper mapper,
            IMemoryCache cache)
        {
            _currencyService = currencyService;
            _mapper = mapper;
            _cache = cache;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {
            string cacheKey = $"Currency_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out PagedResponse<CurrencyResponseDto>? response))
            {
                var result = await _currencyService.GetAllAsync(pageNumber, pageSize);

                response = new PagedResponse<CurrencyResponseDto>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalRecords = result.TotalRecords,
                    TotalPages = result.TotalPages,
                    Data = _mapper.Map<List<CurrencyResponseDto>>(result.Data)
                };

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string cacheKey = $"Currency_{id}";

            if (!_cache.TryGetValue(cacheKey, out CurrencyResponseDto? response))
            {
                var currency = await _currencyService.GetByIdAsync(id);

                if (currency == null)
                    return NotFound("Currency not found.");

                response = _mapper.Map<CurrencyResponseDto>(currency);

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CurrencyRequestDto dto)
        {
            var currency = _mapper.Map<Currency>(dto);

            var createdCurrency = await _currencyService.CreateAsync(currency);

            _cache.Remove("Currency_1_10");

            var response = _mapper.Map<CurrencyResponseDto>(createdCurrency);

            return CreatedAtAction(nameof(GetById),
                new { id = response.CurrencyId },
                response);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            CurrencyRequestDto dto)
        {
            var currency = _mapper.Map<Currency>(dto);

            var updatedCurrency = await _currencyService.UpdateAsync(id, currency);

            if (updatedCurrency == null)
                return NotFound("Currency not found.");

            _cache.Remove($"Currency_{id}");
            _cache.Remove("Currency_1_10");

            var response = _mapper.Map<CurrencyResponseDto>(updatedCurrency);

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _currencyService.DeleteAsync(id);

                if (!result)
                    return NotFound("Currency not found.");

                _cache.Remove($"Currency_{id}");
                _cache.Remove("Currency_1_10");

                return Ok("Currency deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}