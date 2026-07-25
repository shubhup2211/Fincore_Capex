namespace Fincore.Application.DTO.Payment.APInvoice.Responses
{
    public class APInvoiceResponseDto
    {
        public int APInvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public int VendorId { get; set; }

        public string VendorName { get; set; } = string.Empty;

        public int PurchaseOrderId { get; set; }

        public string PurchaseOrderNumber { get; set; } = string.Empty;

        public int GRNId { get; set; }

        public string GRNNumber { get; set; } = string.Empty;

        public int? WorkOrderId { get; set; }

        public string? WorkOrderNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }

        public string? InvoiceFile { get; set; }

        public string ApprovalStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}