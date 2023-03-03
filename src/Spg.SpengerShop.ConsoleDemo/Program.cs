using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spg.SpengerShop.Application.Services;
using Spg.SpengerShop.ConsoleDemo;
using Spg.SpengerShop.DbExtensions;
using Spg.SpengerShop.Domain.Exceptions;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using Spg.SpengerShop.Repository.Repositories;
using System;

// Configuration
ConfigurationBuilder builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", optional: false);
var configuration = builder.Build();

// Services into DI-Container
IServiceCollection services = new ServiceCollection();
services.AddTransient<IAddUpdateableProductService, ProductService>();
services.AddTransient<IReadOnlyProductService, ProductService>();
services.AddTransient<IProductRepository, ProductRepository>();

services.AddLogging(logging => logging.AddConsole());

string? connectionString = configuration.GetConnectionString("Database");
services.ConfigureSqLite(connectionString!);

// Build Service Provider
IServiceProvider? _serviceProvider = services.BuildServiceProvider();

ILogger logger = _serviceProvider.GetRequiredService<ILoggerFactory>()
    .CreateLogger<Program>();

IAddUpdateableProductService productService = _serviceProvider.GetService<IAddUpdateableProductService>()
    ?? throw new MiddlewareWrongConfigurationException(logger, "");

ProductOrchistration bl = new ProductOrchistration(productService, logger);
bl.DoSomething();
