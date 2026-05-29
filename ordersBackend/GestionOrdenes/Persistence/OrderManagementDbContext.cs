using Microsoft.EntityFrameworkCore;
using OrderManagement.Models.Entities;

namespace OrderManagement.Persistence;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            e.Property(x => x.Role).HasColumnName("role").HasMaxLength(20).HasDefaultValue("admin");
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("clients");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            e.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
            e.Property(x => x.FcmToken).HasColumnName("fcm_token").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Driver>(e =>
        {
            e.ToTable("drivers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            e.Property(x => x.Vehicle).HasColumnName("vehicle").HasMaxLength(100).IsRequired();
            e.Property(x => x.Plates).HasColumnName("plates").HasMaxLength(20).IsRequired();
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            e.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
            e.Property(x => x.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");
            e.HasIndex(x => x.Plates).IsUnique();
        });

        modelBuilder.Entity<Delivery>(e =>
        {
            e.ToTable("deliveries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
            e.Property(x => x.DriverId).HasColumnName("driver_id").IsRequired();
            e.Property(x => x.Origin).HasColumnName("origin").HasMaxLength(300).IsRequired();
            e.Property(x => x.Destination).HasColumnName("destination").HasMaxLength(300).IsRequired();
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("pending");
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");

            e.HasOne(x => x.Client)
                .WithMany(x => x.Deliveries)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Driver)
                .WithMany(x => x.Deliveries)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("notifications");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
            e.Property(x => x.DeliveryId).HasColumnName("delivery_id");
            e.Property(x => x.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            e.Property(x => x.Message).HasColumnName("message").HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.IsRead).HasColumnName("is_read").HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            e.HasOne(x => x.Client)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Delivery)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.DeliveryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
