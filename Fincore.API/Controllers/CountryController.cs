using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryService.GetAllAsync();
            return Ok(countries);
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _countryService.GetByIdAsync(id);

            if (country == null)
                return NotFound("Country not found.");

            return Ok(country);
        }

        // POST: api/Country
        [HttpPost]
        public async Task<IActionResult> Create(CountryRequestDto dto)
        {
            var country = new Country
            {
                CountryCode = dto.CountryCode,
                CountryName = dto.CountryName,
                CurrencyId = dto.CurrencyId
            };

            var result = await _countryService.CreateAsync(country);

            return CreatedAtAction(nameof(GetById), new { id = result.CountryId }, result);
        }

        // PUT: api/Country/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CountryRequestDto dto)
        {
            var country = new Country
            {
                CountryCode = dto.CountryCode,
                CountryName = dto.CountryName,
                CurrencyId = dto.CurrencyId
            };

            var result = await _countryService.UpdateAsync(id, country);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _countryService.DeleteAsync(id);

                if (!result)
                    return NotFound();

                return Ok("Country deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}