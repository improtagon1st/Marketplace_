using System;
using System.Collections.Generic;

namespace MarketplaceAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public int? PickupPointId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> OrderPickedUpByWorkers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    public virtual PickupPoint? PickupPoint { get; set; }
}
