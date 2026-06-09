using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class AuditLog
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
