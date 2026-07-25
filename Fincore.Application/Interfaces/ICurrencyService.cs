using Fincore.Application.CommonHelper;
using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<PagedResponse<Currency>> GetAllAsync(int pageNumber, int pageSize);

        Task<Currency?> GetByIdAsync(int id);

        Task<Currency> CreateAsync(Currency currency);

        Task<Currency?> UpdateAsync(int id, Currency currency);

        Task<bool> DeleteAsync(int id);
    }
}