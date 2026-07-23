using Fincore.Application.DTO.Payment;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Infrastructure.CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntryController : ControllerBase
    {
        IJournalEntryService _service;
        public JournalEntryController(IJournalEntryService service)
        {
            this._service = service;
        }


        [HttpPost]
        public async Task<IActionResult> AddJournalEntryAsync([FromBody] JournalEntryPostDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        ApiResponseHelper.Failure<object>(
                            "Invalid Journal Entry Data",
                            "VALIDATION_ERROR",
                            "Please provide valid journal entry details."
                        ));
                }

                await _service.AddJournalEntryAsync(dto);

                return Ok(
                    ApiResponseHelper.SuccessRes(
                        dto,
                        "Journal Entry Added Successfully"
                    ));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        "Failed to Add Journal Entry",
                        "SERVER_ERROR",
                        ex.InnerException?.InnerException?.Message
                        ?? ex.InnerException?.Message
                        ?? ex.Message
                    ));
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllJournalEntries(int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _service.GetAllJournalEntries(page, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<List<JournalEntryGetDTO>>(
                        "Failed to fetch Journal Entries",
                        "SERVER_ERROR",
                        ex.Message
                    ));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJournalEntry(int id)
        {
            try
            {
                await _service.DeleteJournalEntryAsync(id);

                return Ok(
                    ApiResponseHelper.SuccessRes(
                        $"Journal Entry deleted successfully : {id}"
                    ));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        "Journal Entry deletion failed!",
                        "DELETE_JOURNAL_ENTRY_FAILED",
                        ex.Message
                    ));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetJournalEntryById(int id)
        {
            try
            {
                var result = await _service.GetJournalEntryById(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        $"Failed to fetch Journal Entry by Id : {id}",
                        "FAILED_FETCH_JOURNAL_ENTRY",
                        ex.Message
                    ));
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJournalEntry(int id, [FromBody] JournalEntryUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        ApiResponseHelper.Failure<object>(
                            "Invalid Journal Entry Data",
                            "VALIDATION_ERROR",
                            "Please provide valid journal entry details."
                        ));
                }

                await _service.UpdateJournalEntryAsync(id, dto);

                return Ok(
                    ApiResponseHelper.SuccessRes(
                        dto,
                        "Journal Entry Updated Successfully"
                    ));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        "Failed to Update Journal Entry",
                        "SERVER_ERROR",
                        ex.Message
                    ));
            }
        }
    }
}
