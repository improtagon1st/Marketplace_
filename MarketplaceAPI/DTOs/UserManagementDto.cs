using System;

namespace MarketplaceAPI.DTOs
{
    public class WorkerDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int? PickupPointId { get; set; }
        public string PickupPointName { get; set; }
    }

    public class CreateWorkerRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int? PickupPointId { get; set; }
    }

    public class UpdateWorkerRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int? PickupPointId { get; set; }
        public string? NewPassword { get; set; } 
    }
}