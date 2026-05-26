using Microsoft.EntityFrameworkCore;
using OrderManagement.Persistence.Entities;

namespace OrderManagement.Persistence;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
    public DbSet<DriverEntity> Drivers => Set<DriverEntity>();
    public DbSet<DeliveryEntity> Deliveries => Set<DeliveryEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("uq_users_email");
        });

        modelBuilder.Entity<CustomerEntity>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
            entity.Property(x => x.FcmToken).HasColumnName("fcm_token").HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            entity.HasIndex(x => x.Email).IsUnique().HasDatabaseName("uq_customers_email");
        });

        modelBuilder.Entity<DriverEntity>(entity =>
        {
            entity.ToTable("drivers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Vehicle).HasColumnName("vehicle").HasMaxLength(100).IsRequired();
            entity.Property(x => x.LicensePlate).HasColumnName("license_plate").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            entity.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
            entity.Property(x => x.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            entity.HasIndex(x => x.LicensePlate).IsUnique().HasDatabaseName("uq_drivers_license_plate");
        });

        modelBuilder.Entity<DeliveryEntity>(entity =>
        {
            entity.ToTable("deliveries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
            entity.Property(x => x.DriverId).HasColumnName("driver_id").IsRequired();
            entity.Property(x => x.Origin).HasColumnName("origin").HasMaxLength(300).IsRequired();
            entity.Property(x => x.Destination).HasColumnName("destination").HasMaxLength(300).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.Status).HasDatabaseName("idx_deliveries_status");
            entity.HasIndex(x => x.CustomerId).HasDatabaseName("idx_deliveries_customer_id");
            entity.HasIndex(x => x.DriverId).HasDatabaseName("idx_deliveries_driver_id");

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Deliveries)
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("fk_deliveries_customer")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Driver)
                .WithMany(x => x.Deliveries)
                .HasForeignKey(x => x.DriverId)
                .HasConstraintName("fk_deliveries_driver")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(x => x.CustomerId).HasColumnName("customer_id").IsRequired();
            entity.Property(x => x.DeliveryId).HasColumnName("delivery_id");
            entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Message).HasColumnName("message").HasColumnType("text").IsRequired();
            entity.Property(x => x.IsRead).HasColumnName("is_read").HasDefaultValue(false).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.HasIndex(x => x.CustomerId).HasDatabaseName("idx_notifications_customer_id");
            entity.HasIndex(x => x.IsRead).HasDatabaseName("idx_notifications_is_read");

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("fk_notifications_customer")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Delivery)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.DeliveryId)
                .HasConstraintName("fk_notifications_delivery")
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
