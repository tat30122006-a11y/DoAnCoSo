using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<InboundOrder> InboundOrderCreatedByNavigations { get; set; } = new List<InboundOrder>();

    public virtual ICollection<InboundOrder> InboundOrderProcessedByNavigations { get; set; } = new List<InboundOrder>();

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OutboundOrder> OutboundOrderCreatedByNavigations { get; set; } = new List<OutboundOrder>();

    public virtual ICollection<OutboundOrder> OutboundOrderProcessedByNavigations { get; set; } = new List<OutboundOrder>();

    public virtual ICollection<TransferOrder> TransferOrderCreatedByNavigations { get; set; } = new List<TransferOrder>();

    public virtual ICollection<TransferOrder> TransferOrderProcessedByNavigations { get; set; } = new List<TransferOrder>();
}
