using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<InboundOrder> InboundOrders { get; set; } = new List<InboundOrder>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OutboundOrder> OutboundOrders { get; set; } = new List<OutboundOrder>();

    public virtual ICollection<TransferOrder> TransferOrderFromWarehouses { get; set; } = new List<TransferOrder>();

    public virtual ICollection<TransferOrder> TransferOrderToWarehouses { get; set; } = new List<TransferOrder>();
}
