namespace MarketplaceWPF.Helpers
{
    public static class UserSession
    {
        public static string Token { get; set; }
        public static Guid UserId { get; set; }
        public static string Email { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }
        public static int? PickupPointId { get; set; } 

        public static bool IsCustomer => Role == "Customer";
        public static bool IsWorker => Role == "PickupPointWorker";
        public static bool IsAdmin => Role == "Admin";
    }
}