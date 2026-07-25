namespace Fincore.Application.DTO.MasterTable
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int? MasterTypeId { get; set; }

        public string? MasterTypeName { get; set; }

        public byte IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }
    }
}