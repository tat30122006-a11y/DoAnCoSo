using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OutboundOrder> OutboundOrders { get; set; } = new List<OutboundOrder>();
}
