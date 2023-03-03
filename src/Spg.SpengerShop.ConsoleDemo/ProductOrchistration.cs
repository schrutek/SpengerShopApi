using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                _productService.Create(new Product("Testprodukt 3", 20, "0123456789123", "Material 3", DateTime.Now.AddDays(30), null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
