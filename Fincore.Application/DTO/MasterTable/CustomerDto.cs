namespace Fincore.Application.DTO.MasterTable
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

        public string CustomerCode { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public byte IsActive { get; set; }
    }
}