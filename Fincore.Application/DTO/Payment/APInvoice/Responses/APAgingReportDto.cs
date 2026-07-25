namespace Fincore.Application.DTO.Payment.APInvoice.Responses
{
    public class APAgingReportDto
    {
        public int Current { get; set; }

        public int Days1To30 { get; set; }

        public int Days31To60 { get; set; }

        public int Days61To90 { get; set; }

        public int Above90Days { get; set; }

        public decimal TotalOutstandingAmount { get; set; }
    }
}