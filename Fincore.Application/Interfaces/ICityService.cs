using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task<City> CreateAsync(City city);
        Task<City?> UpdateAsync(int id, City city);
        Task<bool> DeleteAsync(int id);
    }
}