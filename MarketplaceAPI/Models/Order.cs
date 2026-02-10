using System;
using System.Collections.Generic;

namespace MarketplaceAPI.Models;

public partial class Order
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int PickupPointId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public string Qrcode { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? PickedUpAt { get; set; }

    public Guid? PickedUpByWorkerId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? PickedUpByWorker { get; set; }

    public virtual PickupPoint PickupPoint { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
