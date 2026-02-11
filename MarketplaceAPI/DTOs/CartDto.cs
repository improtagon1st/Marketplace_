using System;
using System.Collections.Generic;

namespace MarketplaceAPI.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int AvailableStock { get; set; }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemRequest
    {
        public int Quantity { get; set; }
    }

    public class CheckoutRequest
    {
        public int PickupPointId { get; set; }
    }
}