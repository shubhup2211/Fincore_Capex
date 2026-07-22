using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Domain.Models;


namespace Fincore.Application.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country?> GetByIdAsync(int id);
        Task<Country> CreateAsync(Country country);
        Task<Country?> UpdateAsync(int id, Country country);
        Task<bool> DeleteAsync(int id);
    }
}