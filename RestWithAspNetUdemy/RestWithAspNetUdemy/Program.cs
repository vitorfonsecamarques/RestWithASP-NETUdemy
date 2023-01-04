using EvolveDb;
using Microsoft.EntityFrameworkCore;
using RestWithAspNetUdemy.Business;
using RestWithAspNetUdemy.Business.Implementations;
using RestWithAspNetUdemy.Model.Context;
using RestWithAspNetUdemy.Repository.Generic;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var settings = builder.Configuration.GetSection("MySQLConnection:MySQLConnectionString");
var connectionString = settings.Value;

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Services.AddMvc(options =>
{
    options.RespectBrowserAcceptHeader = true;

    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
    options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
})
.AddXmlSerializerFormatters();

builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//Versioning API
builder.Services.AddApiVersioning();

//Dependency Injection
builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

if (environment.IsDevelopment())
{
    MigrateDatabase(connectionString);
}

void MigrateDatabase(string connection)
{
	try
	{
		var evolveConnection = new MySql.Data.MySqlClient.MySqlConnection(connection);
		var evolve = new Evolve(evolveConnection, msg => Log.Information(msg)) 
		{
			Locations = new List<string> { "db/migrations", "db/dataset" },
			IsEraseDisabled = true,
		};

		evolve.Migrate();
	}
	catch (Exception ex)
	{
		Log.Error("Database migration failed", ex);
		throw;
	}
}

app.MapControllers();

app.Run();
