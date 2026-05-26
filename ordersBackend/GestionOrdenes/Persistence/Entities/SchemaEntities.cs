namespace OrderManagement.Persistence.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CustomerEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? FcmToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<DeliveryEntity> Deliveries { get; set; } = new List<DeliveryEntity>();
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
}

public class DriverEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<DeliveryEntity> Deliveries { get; set; } = new List<DeliveryEntity>();
}

public class DeliveryEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int DriverId { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public CustomerEntity Customer { get; set; } = null!;
    public DriverEntity Driver { get; set; } = null!;
    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
}

public class NotificationEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? DeliveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public CustomerEntity Customer { get; set; } = null!;
    public DeliveryEntity? Delivery { get; set; }
}
