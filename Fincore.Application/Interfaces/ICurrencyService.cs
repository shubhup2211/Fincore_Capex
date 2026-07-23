using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(int id);
        Task<Currency> CreateAsync(Currency currency);
        Task<Currency?> UpdateAsync(int id, Currency currency);
        Task<bool> DeleteAsync(int id);
    }
}