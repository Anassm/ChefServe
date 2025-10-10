using Microsoft.EntityFrameworkCore;
using ChefServe.Infrastructure.Data;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Services;
using ChefServe.Core.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Data Source=ChefServe.Infrastructure/Data/database.db";

// Add services to the container.
builder.Services.AddDbContext<ChefServeDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IFileService, FileService>();


var app = builder.Build();

// Run migrations and seed DB
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChefServeDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
