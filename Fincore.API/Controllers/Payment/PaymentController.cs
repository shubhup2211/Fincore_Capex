using Fincore.Application.DTO;
using Fincore.Application.DTO.Payment;
using Fincore.Application.Interfaces.IPayment;
using Fincore.Domain.Enums;
using Fincore.Infrastructure.CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fincore.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        IPaymentService _service;
        public PaymentController(IPaymentService _service)
        {
            this._service = _service;
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentAsync([FromBody] PaymentPostDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        ApiResponseHelper.Failure<object>(
                            "Invalid Payment Data",
                            "VALIDATION_ERROR",
                            "Please provide valid payment details."
                        )
                    );
                }

                await _service.AddPaymentAsync(dto);

                return Ok(
                    ApiResponseHelper.SuccessRes(
                        dto,
                        "Payment Added Successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        "Failed to Add Payment",
                        "SERVER_ERROR",
                        ex.InnerException?.InnerException?.Message
                        ?? ex.InnerException?.Message
                        ?? ex.Message
                    )
                );
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayment(int page=1, int pageSize = 1)
        {
            try
            {
                var result = await _service.GetAllPayment(page, pageSize);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<List<PaymentGetDTO>>(
                            "Invalid Payment Data",
                            "VALIDATION_ERROR",
                            ex.Message
                        ));

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var result = await _service.GetPaymentById(id);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ApiResponseHelper.Failure<object>(
                    $"Failed to Fetch the data by id : {id}",
                    "FAILED_FETCH_PAYMENT",
                    ex.Message
                    
                    ));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(
                        ApiResponseHelper.Failure<object>(
                            "Invalid Payment Data",
                            "VALIDATION_ERROR",
                            "Please provide valid payment details."
                        ));
                }

                await _service.UpdatePaymentAsync(id, dto);

                return Ok(
                    ApiResponseHelper.SuccessRes(
                        dto,
                        "Payment Updated Successfully"
                    ));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<object>(
                        "Failed to Update Payment",
                        "SERVER_ERROR",
                        ex.Message
                    ));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                await _service.DeletePaymentAsync(id);
                return Ok(ApiResponseHelper.SuccessRes($"Payment Record deleted successfully : {id}"));
            }
            catch(Exception ex )
            {
                return BadRequest(ApiResponseHelper.Failure<object>("Payment deletion failed!", "DELETE_PAYMENT_FAILED", ex.Message));
            }
        }

        [HttpGet("type/{paymentType}")]
        public async Task<IActionResult> GetPaymentByType(
    PaymentType paymentType,
    int page = 1,
    int pageSize = 10)
        {
            try
            {
                var result = await _service.GetPaymentType(paymentType, page, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<List<PaymentGetDTO>>(
                        "Failed to fetch payments.",
                        "SERVER_ERROR",
                        ex.Message
                    )
                );
            }
        }


        [HttpGet("status/{ps}")]
        public async Task<IActionResult> GetPaymentStatus(
    PaymentStatus ps,
    int page = 1,
    int pageSize = 10)
        {
            try
            {
                var result = await _service.GetPaymentStatus(ps, page, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponseHelper.Failure<List<PaymentGetDTO>>(
                        "Failed to fetch payments.",
                        "SERVER_ERROR",
                        ex.Message
                    )
                );
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> PaymentApproval(int id)
        {
            try
            {
                await _service.UpdateApproval(id);
                return Ok(ApiResponseHelper.SuccessRes("Pyament Approved "));
            }
            catch(Exception e)
            {
                return BadRequest(ApiResponseHelper.Failure<object>("Failed to Approve","APPROVE_FAILED",e.Message));
            }
        }

        [HttpPut("{id}/reconcile")]
        public async Task<IActionResult> PaymentReconcile(int id)
        {
            try
            {
                await _service.UpdateReconcile(id);
                return Ok(ApiResponseHelper.SuccessRes("Payment Done "));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseHelper.Failure<object>("Failed to Pay", "PAYMENT_FAILED", e.Message));
            }
        }





    }
}
