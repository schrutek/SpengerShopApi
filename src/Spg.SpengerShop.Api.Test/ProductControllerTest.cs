using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Spg.SpengerShop.Api.Controllers;
using Spg.SpengerShop.Api.Test.Helpers;
using Spg.SpengerShop.Application.Services;
using Spg.SpengerShop.Application.Validators;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Spg.SpengerShop.Api.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IReadOnlyProductService> _productServiceMock = new Mock<IReadOnlyProductService>();
        private readonly Mock<IAddUpdateableProductService> _addUpdateableProductService = new Mock<IAddUpdateableProductService>();
        private readonly Mock<IValidator<NewProductDto>> _newProductDtoValidator = new Mock<IValidator<NewProductDto>>();

        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7287/api") };

        [Fact]
        public void Create_SuccessTest()
        {
            // Arrange
            Product? product = null;
            _addUpdateableProductService.Setup(r => r.Create(It.IsAny<Product>()))
                .Callback<Product>(x => product = x);

            NewProductDto newProduct = new NewProductDto() { Name = "Testprodukt", Ean13 = "1234567890121", ExpiryDate = DateTime.Now };

            ProductsController unitToTest = new ProductsController(
                _productServiceMock.Object, 
                _addUpdateableProductService.Object, 
                _newProductDtoValidator.Object);
            
            // Act
            unitToTest.Create(newProduct);
            _addUpdateableProductService.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);

            // Assert
            Assert.Equal(product?.Name, newProduct.Name);
        }

        [Fact]
        public void Create_NeverExecutes()
        {
            // Arrange
            NewProductDto newProduct = new NewProductDto() { Name = "", Ean13 = "1234567890121", ExpiryDate = DateTime.Now };
            ProductsController unitToTest = new ProductsController(
                _productServiceMock.Object, 
                _addUpdateableProductService.Object, 
                _newProductDtoValidator.Object);

            unitToTest.ModelState.AddModelError("Name", "Name is required");

            // Act
            IActionResult result = unitToTest.Create(newProduct);

            // Assert
            //_addUpdateableProductService.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void Create_NeverExecutes_FluentValidation()
        {
            // Arrange
            NewProductDto newProduct = new NewProductDto() { Name = "", Ean13 = "1234567890121", ExpiryDate = DateTime.Now };
            ProductsController unitToTest = new ProductsController(
                _productServiceMock.Object,
                _addUpdateableProductService.Object,
                _newProductDtoValidator.Object);

            _newProductDtoValidator.Setup(v => v.Validate(newProduct))
                .Returns(new FluentValidation.Results.ValidationResult(
                    new List<FluentValidation.Results.ValidationFailure>()
                    {
                        new FluentValidation.Results.ValidationFailure("Name", "Name is required (Fluent!)")
                    }));

            // Act
            IActionResult result = unitToTest.Create(newProduct);

            // Assert
            //_addUpdateableProductService.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
    }
}