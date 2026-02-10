namespace MarketplaceAPI.DTOs
{
    public class CreateOrderRequest
    {
        public int PickupPointId { get; set; }
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}