using Fincore.Domain.Models;

namespace Fincore.Application.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<State>> GetAllAsync();
        Task<State?> GetByIdAsync(int id);
        Task<State> CreateAsync(State state);
        Task<State?> UpdateAsync(int id, State state);
        Task<bool> DeleteAsync(int id);
    }
}