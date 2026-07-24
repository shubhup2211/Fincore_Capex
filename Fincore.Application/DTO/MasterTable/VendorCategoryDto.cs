namespace Fincore.Application.DTO.MasterTable
{
    public class VendorCategoryDto
    {
        public int VendorCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }

        public string CreatedByName { get; set; }

        public int ModifiedBy { get; set; }

        public string ModifiedByName { get; set; }
    }
}