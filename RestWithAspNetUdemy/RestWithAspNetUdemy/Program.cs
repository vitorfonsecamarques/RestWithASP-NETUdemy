using EvolveDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestWithAspNetUdemy.Business;
using RestWithAspNetUdemy.Business.Implementations;
using RestWithAspNetUdemy.Configurations;
using RestWithAspNetUdemy.Hypermedia.Enricher;
using RestWithAspNetUdemy.Hypermedia.Filters;
using RestWithAspNetUdemy.Model.Context;
using RestWithAspNetUdemy.Repository;
using RestWithAspNetUdemy.Repository.Generic;
using RestWithAspNetUdemy.Services;
using RestWithAspNetUdemy.Services.Implementations;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var tokenConfigurations = new TokenConfiguration();

new ConfigureFromConfigurationOptions<TokenConfiguration>(
        builder.Configuration.GetSection("TokenConfigurations")
	).Configure(tokenConfigurations);

builder.Services.AddSingleton(tokenConfigurations);
builder.Services.AddAuthentication(options => 
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
	options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = tokenConfigurations.Issuer,
		ValidAudience = tokenConfigurations.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
	};
});

builder.Services.AddAuthorization(auth =>
{
	auth.AddPolicy("Bearer", new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
		.RequireAuthenticatedUser().Build());
});

builder.Services.AddCors(options => options.AddDefaultPolicy(builder2 => 
{
	builder2.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader();
}));

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

var filterOptions = new HyperMediaFilterOptions();
filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());
filterOptions.ContentResponseEnricherList.Add(new BookEnricher());

builder.Services.AddSingleton(filterOptions);

builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//Versioning API
builder.Services.AddApiVersioning();
builder.Services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1",
		new Microsoft.OpenApi.Models.OpenApiInfo
		{
			Title = "REST API's From 0 to Azure with ASP.NET Core 5 and Docker",
			Version = "v1",
			Description = "API RESTFUL developed in course 'REST API's From 0 to Azure with ASP.NET Core 5 and Docker'",
			Contact = new Microsoft.OpenApi.Models.OpenApiContact
			{
				Name = "Vitor Fonseca",
				Url = new Uri("https://github.com/vitorfonsecamarques")
			}
        });
});

//Dependency Injection
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
builder.Services.AddTransient<ILoginBusiness, LoginBusinessImplementation>();
builder.Services.AddTransient<IFileBusiness, FileBusinessImplementation>();

builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

//if (environment.IsDevelopment())
//{
//	MigrateDatabase(connectionString);
//}

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c => {
	c.SwaggerEndpoint("/swagger/v1/swagger.json", 
		"API RESTFUL developed in course 'REST API's From 0 to Azure with ASP.NET Core 5 and Docker - v1");
});

app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirect("^$", "swagger"));

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
app.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");

app.Run();
