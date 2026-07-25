namespace Fincore.Application.DTO.Payment.APInvoice.Requests
{
    public class APOutstandingFilterDto
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int? VendorId { get; set; }

        public string? Search { get; set; }
    }
}