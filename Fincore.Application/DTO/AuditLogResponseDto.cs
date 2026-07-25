namespace Fincore.Application.DTOs
{
    public class AuditLogResponseDto
    {
        public long AuditLogId { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public long EntityId { get; set; }

        public string OperationType { get; set; } = string.Empty;

        public string? OldData { get; set; }

        public string? NewData { get; set; }

        public int AuditBy { get; set; }

        public DateTime AuditAt { get; set; }
    }
}