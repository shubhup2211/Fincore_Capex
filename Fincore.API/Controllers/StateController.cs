using Fincore.Application.DTOs;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _stateService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var state = await _stateService.GetByIdAsync(id);

            if (state == null)
                return NotFound();

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StateRequestDto dto)
        {
            var state = new State
            {
                StateName = dto.StateName,
                CountryId = dto.CountryId
            };

            var result = await _stateService.CreateAsync(state);

            return CreatedAtAction(nameof(GetById), new { id = result.StateId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StateRequestDto dto)
        {
            var state = new State
            {
                StateName = dto.StateName,
                CountryId = dto.CountryId
            };

            var result = await _stateService.UpdateAsync(id, state);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _stateService.DeleteAsync(id);

                if (!result)
                    return NotFound();

                return Ok("State deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}