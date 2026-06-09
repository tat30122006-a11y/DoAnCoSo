using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class OutboundOrderDetail
{
    public int Id { get; set; }

    public int OutboundOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual OutboundOrder OutboundOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
