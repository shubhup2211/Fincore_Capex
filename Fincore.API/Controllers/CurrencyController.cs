using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _currencyService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currency = await _currencyService.GetByIdAsync(id);

            if (currency == null)
                return NotFound();

            return Ok(currency);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CurrencyRequest request)
        {
            var currency = new Currency
            {
                CurrencyName = request.CurrencyName,
                Symbol = request.Symbol
            };

            var result = await _currencyService.CreateAsync(currency);

            return CreatedAtAction(nameof(GetById),
                new { id = result.CurrencyId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Currency currency)
        {
            var result = await _currencyService.UpdateAsync(id, currency);

            if (result == null)
                return NotFound();

            return Ok("Currency updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _currencyService.DeleteAsync(id);

                if (!result)
                    return NotFound("Currency not found.");

                return Ok("Currency deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

    }
}