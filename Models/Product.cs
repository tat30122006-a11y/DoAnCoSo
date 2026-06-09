using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<InboundOrderDetail> InboundOrderDetails { get; set; } = new List<InboundOrderDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OutboundOrderDetail> OutboundOrderDetails { get; set; } = new List<OutboundOrderDetail>();

    public virtual ICollection<StockLoss> StockLosses { get; set; } = new List<StockLoss>();

    public virtual ICollection<TransferOrderDetail> TransferOrderDetails { get; set; } = new List<TransferOrderDetail>();
}
