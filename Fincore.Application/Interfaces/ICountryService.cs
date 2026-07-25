using Fincore.Application.CommonHelper;
using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface ICountryService
    {
        Task<PagedResponse<Country>> GetAllAsync(int pageNumber, int pageSize);

        Task<Country?> GetByIdAsync(int id);

        Task<Country> CreateAsync(Country country);

        Task<Country?> UpdateAsync(int id, Country country);

        Task<bool> DeleteAsync(int id);
    }
}