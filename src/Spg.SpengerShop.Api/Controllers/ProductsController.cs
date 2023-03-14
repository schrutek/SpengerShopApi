using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spg.SpengerShop.Application.Services.Extended;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Interfaces;

namespace Spg.SpengerShop.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        }

        /// <summary>
        /// Gibt alle Produkte aus der Datenbank zurück.
        /// </summary>
        /// <returns>Eine Liste aller Produkte</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<ProductDto> result = _readOnlyProductService
                    .Load()
                    .UseFilterByName("")
                    .UseFilterByExpiryDate(DateTime.Now, DateTime.Now)
                    .UseFilterByStock(10)
                    .GetDataPaged(1, 2);
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
        public IActionResult GetDetails(int id)
        {
            return Ok(new List<string>() { "A", "B", "C", "D" });
        }

        [HttpPost()]
        [Produces("application/json")]
        //[HasRole()]
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

            int newId = 4711;
            string? url = _linkGenerator.GetUriByAction(HttpContext, action: nameof(GetDetails), values: new { id = newId });

            return Created(url, newProduct);
        }
    }
}
