using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Spg.SpengerShop.Api.Services;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Exceptions;

namespace Spg.SpengerShop.Api.Controllers.V1
{
    /// <summary>
    /// Controller zum Aufrufen der Authentication Services:
    /// Für den Einsatz in WebAPI Projekten:
    ///     user/login: Prüft die Userdaten aus dem Request Body und gibt einen JWT zurück.
    /// Für den Einsatz in Blazor oder MVC Projekten:
    ///     user/loginform: Prüft die x-www-formencoded codierten Userdaten, setzt das Cookie und
    ///                     leitet danach auf die Standardseite (/) um.
    ///     user/logout: Löscht das Cookie und leitet danach auf die Standardseite (/) um.
    ///     
    /// user/register: Liest die Daten aus dem Registrierungsformular und legt den User in der DB an.
    /// </summary>
    [Route("api/v{version:apiVersion}/Account")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly ApiAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// Setzt das AuthService, welches mit
        ///     services.AddScoped<UserService>(services => 
        ///         new UserService(jwtSecret);
        /// in ConfigureServices() registriert wurde.
        /// </summary>
        /// <param name="userService">Registriertes Userservice.</param>
        public AccountController(ApiAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// POST Route für /api/user/login
        /// Generiert den Token für eine JWT basierende Autentifizierung.
        /// </summary>
        /// <remarks>
        /// User: martin
        /// PWD: geheim
        /// </remarks>
        /// <param name="user">Userdaten aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>Token als String oder BadRequest wenn der Benutzer nicht angemeldet werden konnte.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Login(UserCredentialsDto user)
        {
            try
            {
                return await _authService.GenerateToken(user);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// POST Route für /api/user/loginform
        /// Setzt das Cookie für die Authentifizierung, wenn der User angemeldet werden konnte.
        /// Leitet danach auf die Standardseite (/) um.
        /// </summary>
        /// <param name="user">Userdaten aus dem Loginformular.</param>
        /// <returns>Redirect zur Hauptseite.</returns>
        [HttpPost("loginform")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<ActionResult<string>> LoginAsync([FromForm] UserCredentialsDto user)
        {
            await HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsIdentity claimsIdentity;
            if ((claimsIdentity = await _authService.GenerateIdentity(user)) != null)
            {
                // Spezielle Properties (Expires, ...) können als 3. Parameter mit einer
                // AuthenticationProperties Instanz übergeben werden.
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }
            return Redirect("/");
        }

        /// <summary>
        /// POST Route für /api/user/logout
        /// Entfernt das Cookie für die Authentifizierung.
        /// </summary>
        /// <returns>Redirect zur Hauptseite.</returns>
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}