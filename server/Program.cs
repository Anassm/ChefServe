using Microsoft.EntityFrameworkCore;
using ChefServe.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Data Source=ChefServe.Infrastructure/Data/database.db";

// Add services to the container.
builder.Services.AddDbContext<ChefServeDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
