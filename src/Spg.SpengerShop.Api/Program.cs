//using Microsoft.EntityFrameworkCore;
//using Spg.SpengerShop.Infrastructure;

using Asp.Versioning.Conventions;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spg.SpengerShop.Api.Controllers;
using Spg.SpengerShop.Api.Services;
using Spg.SpengerShop.Application.Authentication;
using Spg.SpengerShop.Application.Mock;
using Spg.SpengerShop.Application.Services;
using Spg.SpengerShop.Application.Services.Customers.Commands;
using Spg.SpengerShop.Application.Services.Customers.Queries;
using Spg.SpengerShop.Application.Validators;
using Spg.SpengerShop.DbExtensions;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Filter;
using Spg.SpengerShop.Domain.Helpers;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using Spg.SpengerShop.Infrastructure;
using Spg.SpengerShop.Repository;
using Spg.SpengerShop.Repository.Repositories;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Create services to the container.

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

// Services
builder.Services.AddTransient<IAddUpdateableProductService, ProductService>();
builder.Services.AddTransient<IReadOnlyProductService, ProductService>();

// Repositories
builder.Services.AddTransient<IRepository<Customer>, CustomerRepository>();
builder.Services.AddTransient<IRepositoryBase<Product>, RepositoryBase<Product>>();
builder.Services.AddTransient<IReadOnlyRepositoryBase<Product>, ReadOnlyRepositoryBase<Product>>();
builder.Services.AddTransient<IReadOnlyRepositoryBase<Category>, ReadOnlyRepositoryBase<Category>>();
builder.Services.AddTransient<IReadOnlyRepositoryBase<Customer>, ReadOnlyRepositoryBase<Customer>>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

// MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly()); 
builder.Services.AddTransient<IRequestHandler<GetCustomerByIdQuery, Customer>, GetCustomerByIdQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetFilteredCustomerQuery, IQueryable<Customer>>, GetFilteredCustomerHandler>();
builder.Services.AddTransient<IRequestHandler<CreateCustomerCommand, Customer>, CreateCustomerCommandHandler>();

// AuthService
builder.Services.AddTransient<IAuthService, DbAuthService>();

builder.Services.ConfigureSqLite(connectionString);

// Global Filter
//builder.Services.AddControllers(config =>
//{
//    config.Filters.Create(new ValidationFilterAttribute());
//});
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// API-Versioning
// NuGet: Microsoft.AspNetCore.Mvc.Versioning
//        Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"));
});
builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://localhost:7042");
        policy.WithHeaders("ACCESS-CONTROL-ALLOW-ORIGIN", "CONTENT-TYPE");
    });
});

// Create FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<IValidator<NewCustomerDto>, NewCustomerDtoValidator>();
builder.Services.AddTransient<IValidator<NewProductDto>, NewProductDtoValidator>();

// Controller Filter, Action Filter
builder.Services.AddScoped<ValidationFilterAttribute>();


// Authentication
// Soll ein gespeichertes Secret verwendet werden, kann folgende Zeite statt dessen
// verwendet werden:
string jwtSecret = builder.Configuration["AppSettings:Secret"] ?? ApiAuthService.GenerateRandom(1024);

// JWT aktivieren, aber nicht standardmäßig aktivieren. Daher muss beim Controller
//     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// geschrieben werden. Wird nur eine API bereitgestellt, kann dieser Parameter auf
// true gesetzt und Cookies natürlich deaktiviert werden.
builder.Services.AddJwtAuthentication(jwtSecret, setDefault: true);

// Cookies aktivieren. Dies ist für Blazor oder MVC Applikationen gedacht.
builder.Services.AddCookieAuthentication(setDefault: false);

// Instanzieren des Userservices mit einer Factorymethode. Diese übergibt das gespeicherte
// Secret.
//builder.Services.AddTransient<ApiAuthService>();
// oder folgende zeile mit einem Secret aus der appsettings.json
builder.Services.AddScoped<ApiAuthService>(services =>
    new ApiAuthService(jwtSecret, new DbAuthService(null)));

// Authentication

builder.Services.AddHttpContextAccessor();



DbContextOptions options = new DbContextOptionsBuilder()
.UseSqlite(connectionString)
.Options;
SpengerShopContext db = new SpengerShopContext(options);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
db.Seed();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2");
    });
}

app.UseHttpsRedirection();
app.UseCors("myAllowSpecificOrigins");

// Muss NACH UseRouting() und VOR UseEndpoints() stehen.
app.UseAuthentication();
app.UseAuthorization();

app.UsePathBase("/myapp");
app.MapControllers();

app.MapGet("/api", (HttpContext context) =>
{
    UriBuilder uriBuilder = new UriBuilder(context.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? -1, "api");
    LinkCollectionWrapper<MainDto> mainWrapper = new LinkCollectionWrapper<MainDto>(null!);
    return CreateLinksForMain(uriBuilder, "v1", mainWrapper);
});
app.MapGet("/api/v1", (HttpContext context) =>
{
    UriBuilder uriBuilder = new UriBuilder(context.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? -1, "api");
    LinkCollectionWrapper<MainDto> mainWrapper = new LinkCollectionWrapper<MainDto>(null!);
    return CreateLinksForMainV1(uriBuilder, "v1", mainWrapper);
});
app.MapGet("/api/v2", (HttpContext context) =>
{
    UriBuilder uriBuilder = new UriBuilder(context.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? -1, "api");
    LinkCollectionWrapper<MainDto> mainWrapper = new LinkCollectionWrapper<MainDto>(null!);
    return CreateLinksForMainV2(uriBuilder, "v2", mainWrapper);
});

app.Run();

LinkCollectionWrapper<TEntity> CreateLinksForMain<TEntity>(UriBuilder url, string version, LinkCollectionWrapper<TEntity> wrapper)
{
    wrapper.Links = new List<Link>() {
            new Link($"{url}/v1",
                "self",
                "GET"),
            new Link($"{url}/v2",
                "self",
                "GET")
    };
    return wrapper;
}
LinkCollectionWrapper<TEntity> CreateLinksForMainV1<TEntity>(UriBuilder url, string version, LinkCollectionWrapper<TEntity> wrapper)
{
    wrapper.Links = new List<Link>() {
            new Link($"{url}/{version}/Products",
                "self",
                "GET"),
            new Link($"{url}/{version}/Customers",
                "self",
                "GET")
    };
    return wrapper;
}
LinkCollectionWrapper<TEntity> CreateLinksForMainV2<TEntity>(UriBuilder url, string version, LinkCollectionWrapper<TEntity> wrapper)
{
    wrapper.Links = new List<Link>() {
            new Link($"{url}/{version}/Customers",
                "self",
                "GET")
    };
    return wrapper;
}

public partial class Program { }