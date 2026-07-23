using Fincore.Application.Interfaces;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly AppDbContext _context;

        public CurrencyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await _context.Currencies.ToListAsync();
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            return await _context.Currencies.FindAsync(id);
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return currency;
        }

        public async Task<Currency?> UpdateAsync(int id, Currency currency)
        {
            var existingCurrency = await _context.Currencies.FindAsync(id);

            if (existingCurrency == null)
                return null;

            existingCurrency.CurrencyName = currency.CurrencyName;
            existingCurrency.Symbol = currency.Symbol;

            await _context.SaveChangesAsync();

            return existingCurrency;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
                return false;


            bool countryExists = await _context.Countries
                .AnyAsync(c => c.CurrencyId == id);

            if (countryExists)
            {
                throw new InvalidOperationException(
                    "Cannot delete this currency because it is used by countries.");
            }


            _context.Currencies.Remove(currency);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}