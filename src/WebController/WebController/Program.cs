using Microsoft.EntityFrameworkCore;
using WebController.Data;
using WebController.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=ScadaDigitalTwinDB;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Web Services
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Important for API endpoints

// 3. MQTT Background Service
builder.Services.AddHostedService<MqttSubscriberService>();

// 4. CORS (if needed for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable CORS
app.UseCors("AllowAll");

// Map endpoints
app.MapRazorPages();
app.MapControllers(); // Critical for API routes

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // Creates database if it doesn't exist
}

app.Run();