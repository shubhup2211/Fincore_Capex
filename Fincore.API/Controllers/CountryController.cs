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
        public async Task<IActionResult> Create(Country country)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _countryService.CreateAsync(country);

            return CreatedAtAction(nameof(GetById), new { id = result.CountryId }, result);
        }

        // PUT: api/Country/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Country country)
        {
            if (id != country.CountryId)
                return BadRequest("Country Id mismatch.");

            var result = await _countryService.UpdateAsync(id, country);

            if (result == null)
                return NotFound("Country not found.");

            return Ok(result);
        }

        // DELETE: api/Country/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _countryService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Country not found.");

            return Ok("Country deleted successfully.");
        }
    }
}