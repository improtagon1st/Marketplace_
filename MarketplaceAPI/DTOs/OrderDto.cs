namespace MarketplaceAPI.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string PickupPointName { get; set; }
        public string PickupPointAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string QRCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }
}