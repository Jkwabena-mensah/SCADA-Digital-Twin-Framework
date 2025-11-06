using Microsoft.EntityFrameworkCore;
using WebController.Data;
using WebController.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TempDatabase"));
builder.Services.AddHostedService<MqttSubscriberService>();
builder.Services.AddControllers(); // Enable API controllers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll"); // Apply CORS policy
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers(); // Map API endpoints

app.Run();