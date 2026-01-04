using Microsoft.EntityFrameworkCore;
using ChefServe.Infrastructure.Data;
using ChefServe.Core.Interfaces;
using ChefServe.Infrastructure.Services;
using ChefServe.Core.Services;
using ChefServe.API.Middleware;
using Microsoft.AspNetCore.Http.Features;

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
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5175") // your dev frontends
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // required for cookies
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // ~100 MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null; // ~100 MB
});


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
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "openapi/{documentName}.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ChefServe API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.UseAdminAuth();
app.MapControllers();
app.Run();
