using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;

namespace Fincore.Application.Interfaces.IPayment
{
    public interface IJournalEntryService
    {
        Task AddJournalEntryAsync(JournalEntryPostDTO dto);

        Task<ApiResponse<List<JournalEntryGetDTO>>> GetAllJournalEntries(int page, int pageSize);

        Task DeleteJournalEntryAsync(int id);

        Task<ApiResponse<JournalEntryGetDTO>> GetJournalEntryById(int id);

        Task UpdateJournalEntryAsync(int id, JournalEntryUpdateDTO dto);
    }
}
