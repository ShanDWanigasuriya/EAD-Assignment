using EVCharging.WebApi.Infrastructure;
using EVCharging.WebApi.Infrastructure.Repositories;
using EVCharging.WebApi.Services;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add core MVC + JSON serialization support
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Add Swagger (API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EV Charging Station Booking API",
        Version = "v1",
        Description = "Central FAT Service (C# Web API + MongoDB) for EV Charging System"
    });
});

// Add FluentValidation (optional but good)
builder.Services.AddFluentValidationAutoValidation();

// Configure MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbService>();

// Register Repositories (Data Access Layer)
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<OwnerRepository>();
builder.Services.AddSingleton<StationRepository>();
builder.Services.AddSingleton<BookingRepository>();

// Register Services (Business Logic Layer)
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<OwnerService>();
builder.Services.AddSingleton<StationService>();
builder.Services.AddSingleton<BookingService>();

// Enable CORS (allow Web + Mobile clients)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Build the app
var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EV Charging API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Run the application
app.Run();
