using System;

namespace DoAnCoSo.Models.ViewModels
{
    public class AuditLogVM
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public int RecordId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}