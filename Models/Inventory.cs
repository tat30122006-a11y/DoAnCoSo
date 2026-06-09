using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class Inventory
{
    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int MinQuantity { get; set; }

    public int MaxQuantity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
