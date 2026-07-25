using Fincore.Application.CommonHelper;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class StateService : IStateService
    {
        private readonly AppDbContext _context;

        public StateService(AppDbContext context)
        {
            _context = context;
        }

        // Pagination
        public async Task<PagedResponse<State>> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _context.States.CountAsync();

            var states = await _context.States
                .Include(x => x.Country)
                .OrderBy(x => x.StateId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<State>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = states
            };
        }

        public async Task<State?> GetByIdAsync(int id)
        {
            return await _context.States
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.StateId == id);
        }

        public async Task<State> CreateAsync(State state)
        {
            _context.States.Add(state);

            await _context.SaveChangesAsync();

            return state;
        }

        public async Task<State?> UpdateAsync(int id, State state)
        {
            var existingState = await _context.States.FindAsync(id);

            if (existingState == null)
                return null;

            existingState.StateName = state.StateName;
            existingState.CountryId = state.CountryId;

            await _context.SaveChangesAsync();

            return existingState;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var state = await _context.States.FindAsync(id);

            if (state == null)
                return false;

            // Validation
            bool cityExists = await _context.Cities
                .AnyAsync(x => x.StateId == id);

            if (cityExists)
            {
                throw new InvalidOperationException(
                    "Cannot delete this state because it contains cities.");
            }

            _context.States.Remove(state);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}