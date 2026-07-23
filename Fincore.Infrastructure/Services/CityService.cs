using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class CityService : ICityService
    {
        private readonly AppDbContext _context;

        public CityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .Include(c => c.State)
                .ToListAsync();
        }

        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities
                .Include(c => c.State)
                .FirstOrDefaultAsync(c => c.CityId == id);
        }

        public async Task<City> CreateAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<City?> UpdateAsync(int id, City city)
        {
            var existingCity = await _context.Cities.FindAsync(id);

            if (existingCity == null)
                return null;

            existingCity.CityName = city.CityName;
            existingCity.StateId = city.StateId;

            await _context.SaveChangesAsync();

            return existingCity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
                return false;

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}