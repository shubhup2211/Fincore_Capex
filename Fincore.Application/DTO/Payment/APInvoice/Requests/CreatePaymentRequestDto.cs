using System.ComponentModel.DataAnnotations;

namespace Fincore.Application.DTO.Payment.APInvoice.Requests
{
    public class CreatePaymentRequestDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int APInvoiceId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VendorId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}