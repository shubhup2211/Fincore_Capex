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

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.States
                .Include(s => s.Country)
                .ToListAsync();
        }

        public async Task<State?> GetByIdAsync(int id)
        {
            return await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.StateId == id);
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

            var cities = await _context.Cities
                .Where(c => c.StateId == id)
                .ToListAsync();

            if (cities.Any())
            {
                _context.Cities.RemoveRange(cities);
            }

            _context.States.Remove(state);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}