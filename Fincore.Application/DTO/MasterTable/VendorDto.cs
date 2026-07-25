namespace Fincore.Application.DTO.MasterTable
{
    public class VendorDto
    {
        public int VendorId { get; set; }

        public string VendorCode { get; set; }

        public int VendorCategoryId { get; set; }

        public string VendorCategoryName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string BankAccount { get; set; }

        public string PAN { get; set; }

        public decimal? PerformanceScore { get; set; }

        public byte? IsVerified { get; set; }

        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public int ModifiedBy { get; set; }

        public string ModifiedByName { get; set; }
    }
}