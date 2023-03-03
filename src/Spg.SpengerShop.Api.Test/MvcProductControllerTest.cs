using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Spg.SpengerShop.Api.Test.Helpers;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Api.Test
{
    public class MvcProductControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public MvcProductControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task POST_ProductReturn_SuccessAndCorrectContentType()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SpengerShopContext>();

                DatabaseUtilities.ReinitializeDbForTests(db);
            }

            NewProductDto content = new NewProductDto() { Name = "xxxxx", Ean13 = "1234567890121", ExpiryDate = new DateTime(2023, 03, 10) };
            NewProductDto expected = new NewProductDto() { Name = "xxxxx", Ean13 = "1234567890121", ExpiryDate = new DateTime(2023, 03, 10) };
            HttpClient client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("role", "admin");

            var stopwatch = Stopwatch.StartNew();

            // Act
            HttpResponseMessage response = await client.PostAsync("/api/v1/Products", TestHelpers.GetJsonStringContent(content));

            // Assert
            await TestHelpers.AssertResponseWithContentAsync(
                stopwatch,
                response,
                HttpStatusCode.Created,
                expected
            );
        }
    }
}
