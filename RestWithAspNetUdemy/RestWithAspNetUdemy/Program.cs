using Microsoft.EntityFrameworkCore;
using RestWithAspNetUdemy.Model.Context;
using RestWithAspNetUdemy.Services;
using RestWithAspNetUdemy.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var settings = builder.Configuration.GetSection("MySQLConnection:MySQLConnectionString");
var connectionString = settings.Value;

builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//Versioning API
builder.Services.AddApiVersioning();

//Dependency Injection
builder.Services.AddScoped<IPersonService, PersonServiceImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
