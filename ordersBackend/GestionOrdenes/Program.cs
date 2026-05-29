using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.Persistence;
using OrderManagement.Repositories;
using OrderManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddControllers();

// --- Repositories ---
builder.Services.AddScoped<IClientRepository,       ClientRepository>();
builder.Services.AddScoped<IDriverRepository,       DriverRepository>();
builder.Services.AddScoped<IDeliveryRepository,     DeliveryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// --- Services ---
builder.Services.AddScoped<IAuthService,         AuthService>();
builder.Services.AddScoped<IClientService,       ClientService>();
builder.Services.AddScoped<IDriverService,       DriverService>();
builder.Services.AddScoped<IDeliveryService,     DeliveryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// --- JWT ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey   = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings["Issuer"],
            ValidAudience            = jwtSettings["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();

// --- CORS ---
builder.Services.AddCors(options =>
    options.AddPolicy("DevCors", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Description  = "JWT Bearer token",
        In           = ParameterLocation.Header,
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Reference    = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

// --- Firebase ---
var firebaseCredPath = builder.Configuration["Firebase:CredentialsPath"];
if (!string.IsNullOrEmpty(firebaseCredPath) && File.Exists(firebaseCredPath) && FirebaseApp.DefaultInstance is null)
{
#pragma warning disable CS0618
    FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(firebaseCredPath) });
#pragma warning restore CS0618
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1"));
}

app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
