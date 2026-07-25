using Fincore.Application.CommonHelper;
using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class CountryService : ICountryService
    {
        private readonly AppDbContext _context;

        public CountryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<Country>> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _context.Countries.CountAsync();

            var countries = await _context.Countries
                .OrderBy(x => x.CountryId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Country>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = countries
            };
        }

        public async Task<Country?> GetByIdAsync(int id)
        {
            return await _context.Countries.FindAsync(id);
        }

        public async Task<Country> CreateAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }

        public async Task<Country?> UpdateAsync(int id, Country country)
        {
            var existingCountry = await _context.Countries.FindAsync(id);

            if (existingCountry == null)
                return null;

            existingCountry.CountryCode = country.CountryCode;
            existingCountry.CountryName = country.CountryName;
            existingCountry.CurrencyId = country.CurrencyId;

            await _context.SaveChangesAsync();

            return existingCountry;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
                return false;

            bool companyExists = await _context.Companies
                .AnyAsync(c => c.CountryId == id);

            if (companyExists)
                throw new InvalidOperationException(
                    "Cannot delete this country because it is assigned to one or more companies.");

            bool stateExists = await _context.States
                .AnyAsync(s => s.CountryId == id);

            if (stateExists)
              throw new InvalidOperationException(
                   "Cannot delete this country because it has states.");

            _context.Countries.Remove(country);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}