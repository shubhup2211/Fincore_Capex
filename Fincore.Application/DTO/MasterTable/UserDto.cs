namespace Fincore.Application.DTO.MasterTable
{
    public class UserDto
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string UserCategory { get; set; }

        public string Phone { get; set; }

        public DateTime? LastLogin { get; set; }

        public byte IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }
    }
}