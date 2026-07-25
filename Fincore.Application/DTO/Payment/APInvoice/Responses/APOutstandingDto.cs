namespace Fincore.Application.DTO.Payment.APInvoice.Responses
{
    public class APOutstandingDto
    {
        public int APInvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public string VendorName { get; set; } = string.Empty;

        public decimal InvoiceAmount { get; set; }

        public decimal OutstandingAmount { get; set; }

        public DateTime DueDate { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}