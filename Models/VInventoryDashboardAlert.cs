using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class VInventoryDashboardAlert
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int CurrentStock { get; set; }

    public int MinQuantity { get; set; }

    public int MaxQuantity { get; set; }

    public string? StockStatusText { get; set; }
}
