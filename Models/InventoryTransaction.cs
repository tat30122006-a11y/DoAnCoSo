using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class InventoryTransaction
{
    public long Id { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = null!;

    public string ReferenceCode { get; set; } = null!;

    public int QuantityChanged { get; set; }

    public int BeforeQuantity { get; set; }

    public int AfterQuantity { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
