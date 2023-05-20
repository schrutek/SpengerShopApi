using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spg.SpengerShop.Application.Filter;
using Spg.SpengerShop.Application.Services.Extended;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Helpers;
using Spg.SpengerShop.Domain.Interfaces;
using System;

namespace Spg.SpengerShop.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ControllerBase
    {
        private readonly IReadOnlyProductService _readOnlyProductService;
        private readonly IValidator<NewProductDto> _validator;
        private readonly LinkGenerator _linkGenerator;

        public ProductsController(IReadOnlyProductService readOnlyProductService, IValidator<NewProductDto> validator, LinkGenerator linkGenerator)
        {
            _readOnlyProductService = readOnlyProductService;
            _validator = validator;
            _linkGenerator = linkGenerator;

            CheckThisController();
        }

        /// <summary>
        /// Gibt alle Produkte aus der Datenbank zurück.
        /// </summary>
        /// <returns>Eine Liste aller Produkte</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                List<ProductDto> result = _readOnlyProductService
                    .Load()
                    .UseFilterByName("")
                    .UseFilterByExpiryDate(DateTime.Now, DateTime.Now)
                    .UseFilterByStock(10)
                    .GetDataPaged(1, 2)
                    .ToList();

                //return Ok(CreateLinksForGetAllProducts("id", result));
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Hier logging einfügen...
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        [Authorize()] //Roles = "Teacher"
        public IActionResult GetDetails(string name)
        {
            return Ok(new List<string>() { "A", "B", "C", "D" });
        }

        [HttpPost()]
        [Produces("application/json")]
        [AllowAnonymous]
        [HasRole(Role = "admin")]
        public IActionResult Save([FromBody()] NewProductDto newProduct) 
        {
            // bad coding
            //if (string.IsNullOrEmpty(newProduct.Name))
            //{
            //    return BadRequest();
            //}
            //if (newProduct.Name.Length == 0)
            //{
            //    return BadRequest();
            //}
            //if (newProduct.Name.Length > 20)
            //{
            //    return BadRequest();
            //}
            // ...

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            ValidationResult result = _validator.Validate(newProduct);
            if (result.IsValid)
            { }

            var currentPort = HttpContext.Request.Host.Port;

            string newId = "Testproduct 1";
            string? url = _linkGenerator.GetUriByAction(HttpContext, action: nameof(GetDetails), values: new { id = newId });

            return Created(url, newProduct);
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [AllowAnonymous]
        //[HasRole()]
        public IActionResult Delete(string name)
        {
            return Ok();
        }

        private LinkCollectionWrapper<ProductDto> CreateLinksForGetAllProducts(string id, IEnumerable<ProductDto> result)
        {
            foreach (ProductDto dto in result)
            {
                dto.AddLinks(new List<Link>
                {
                    new Link(_linkGenerator.GetUriByAction(HttpContext, action: nameof(GetDetails), values: new { id = dto.Name }),
                        "self",
                        "GET"),
                    new Link(_linkGenerator.GetUriByAction(HttpContext, action: nameof(Delete), values: new { id = dto.Name }),
                        "delete_product",
                        "DELETE")
                });
            }

            LinkCollectionWrapper<ProductDto> wrapper = new LinkCollectionWrapper<ProductDto>(result);
            wrapper.Links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAll)),
                    "self",
                    "GET")
            };
            return wrapper;
        }

        private void CheckThisController()
        {
            var methods = this.GetType().GetMethods();
            var attr = methods[3].GetCustomAttributes(false);
        }
    }
}
