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
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public StateController(
            IStateService stateService,
            IMapper mapper,
            IMemoryCache cache)
        {
            _stateService = stateService;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10)
        {
            string cacheKey = $"State_{pageNumber}_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out PagedResponse<StateResponseDto>? response))
            {
                var result = await _stateService.GetAllAsync(pageNumber, pageSize);

                response = new PagedResponse<StateResponseDto>
                {
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalRecords = result.TotalRecords,
                    TotalPages = result.TotalPages,
                    Data = _mapper.Map<List<StateResponseDto>>(result.Data)
                };

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string cacheKey = $"State_{id}";

            if (!_cache.TryGetValue(cacheKey, out StateResponseDto? response))
            {
                var state = await _stateService.GetByIdAsync(id);

                if (state == null)
                    return NotFound("State not found.");

                response = _mapper.Map<StateResponseDto>(state);

                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StateRequestDto dto)
        {
            var state = _mapper.Map<State>(dto);

            if (state == null)
                return BadRequest("State is null after mapping.");

            return Ok(new
            {
                state.StateName,
                state.CountryId
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StateRequestDto dto)
        {
            var state = _mapper.Map<State>(dto);

            var updatedState = await _stateService.UpdateAsync(id, state);

            if (updatedState == null)
                return NotFound("State not found.");

            _cache.Remove($"State_{id}");
            _cache.Remove("State_1_10");

            var response = _mapper.Map<StateResponseDto>(updatedState);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _stateService.DeleteAsync(id);

                if (!result)
                    return NotFound("State not found.");

                _cache.Remove($"State_{id}");
                _cache.Remove("State_1_10");

                return Ok("State deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}