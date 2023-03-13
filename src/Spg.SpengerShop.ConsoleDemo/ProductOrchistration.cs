using Bogus.DataSets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Exceptions;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.ConsoleDemo
{
    public class ProductOrchistration // Same as a Controller
    {
        private readonly IAddUpdateableProductService _productService;
        private readonly ILogger _logger;

        public ProductOrchistration(IAddUpdateableProductService productService, ILogger logger)
        {
            _productService = productService;
            _logger = logger;

            _logger.LogInformation("Constructor executed!");
        }

        public void DoSomething()
        {
            _logger.LogInformation("DoSomething() executed!");

            try
            {
                _productService.Create(new ProductDto()
                {
                    Name = "Testprodukt 3",
                    Ean13 = "0123456789123",
                    ExpiryDate = DateTime.Now.AddDays(30),
                    DeliveryDate = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
