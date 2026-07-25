namespace Fincore.Application.DTO.Payment.APInvoice.Requests
{
    public class APInvoiceFilterDto
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int? VendorId { get; set; }

        public string? ApprovalStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? Search { get; set; }

        public string SortBy { get; set; } = "CreatedAt";

        public string SortOrder { get; set; } = "desc";
    }
}