using Microsoft.IdentityModel.Tokens;
using Spg.SpengerShop.Application.Authentication;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Exceptions;
using Spg.SpengerShop.Domain.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Spg.SpengerShop.Api.Services
{
    public class ApiAuthService
    {
        private const string _salt = "5Snh3qZNODtDd2Ibsj7irayIl6E1WWmpbvXtcSGlm1o=";
        private readonly byte[] _secret = new byte[0];
        private readonly IAuthService _authService;

        /// <summary>
        /// Konstruktor für die Verwendung ohne JWT.
        /// </summary>
        public ApiAuthService(IAuthService authService)
        {
            _authService = authService;
            _secret = Encoding.ASCII.GetBytes(_salt);
        }

        /// <summary>
        /// Konstruktor mit Secret für die Verwendung mit JWT.
        /// </summary>
        /// <param name="secret">Base64 codierter String für das Secret des JWT.</param>
        public ApiAuthService(string secret, IAuthService authService)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is null or empty.", nameof(secret));
            }
            _secret = Convert.FromBase64String(secret);
            _authService = authService;
        }

        /// <summary>
        /// Prüft, ob der übergebene User existiert und gibt seine Rolle zurück.
        /// TODO: Anpassen der Logik an die eigenen Erfordernisse.
        /// </summary>
        /// <param name="credentials">Benutzername und Passwort, die geprüft werden.</param>
        /// <returns>
        /// Rolle, wenn der Benutzer authentifiziert werden konnte.
        /// Null, wenn der Benutzer nicht authentifiziert werden konnte.
        /// </returns>
        protected virtual async Task<AuthInfos> CheckUser(UserCredentialsDto credentials)
        {
            string hashedPassword = CalculateHash(credentials.Password, _salt);
            return await _authService.CheckIdentity(credentials.UserName, hashedPassword);
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.
        /// Wird eine PupilId übergeben, so wird die Rolle "Pupil" zugewiesen, ansonsten "Teacher".
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        /*
        public async Task<User> CreateUser(UserCredentials credentials)
        {
            string salt = GenerateRandom();
            // Den neuen Userdatensatz erstellen
            User newUser = new User
            {
                U_Name = credentials.Username,
                U_Salt = salt,
                U_Hash = CalculateHash(credentials.Password, salt),
            };
            // Die Rolle des Users zuweisen
            newUser.U_Role = "";
            db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            await db.SaveChangesAsync();
            return newUser;
        }
        */

        public Task<string> GenerateToken(UserCredentialsDto credentials)
        {
            return GenerateToken(credentials, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User.
        /// </summary>
        /// <param name="credentials">Userdaten, die in den Token codiert werden sollen.</param>
        /// <returns>
        /// JSON Web Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public async Task<string> GenerateToken(UserCredentialsDto credentials, TimeSpan lifetime)
        {
            if (credentials is null) { throw new ArgumentNullException(nameof(credentials)); }

            AuthInfos authInfos;
            try
            {
                authInfos = await CheckUser(credentials);
            }
            catch (AuthenticationException) { throw; }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload für den JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                // Benutzername als Typ ClaimTypes.Name.
                new Claim(ClaimTypes.Name, credentials.UserName),
                // ...
                new Claim(ClaimTypes.Surname, authInfos.FirstName),
                new Claim(ClaimTypes.GivenName, authInfos.LastName),
                // Rolle des Benutzer als ClaimTypes.DefaultRoleClaimType
                new Claim(ClaimsIdentity.DefaultRoleClaimType, authInfos.Role),
                }),
                Expires = DateTime.UtcNow + lifetime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Erstellt für den User ein ClaimsIdentity Objekt, wenn der User angemeldet werden konnte.
        /// </summary>
        /// <param name="credentials">Username und Passwort, welches geprüft werden soll.</param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateIdentity(UserCredentialsDto credentials)
        {
            AuthInfos authInfos;
            try
            {
                authInfos = await CheckUser(credentials);
            }
            catch (AuthenticationException) { throw; }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials.UserName),
                new Claim(ClaimTypes.Surname, authInfos.FirstName),
                new Claim(ClaimTypes.GivenName, authInfos.LastName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, authInfos.Role),
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return claimsIdentity;
        }

        /// <summary>
        /// Generiert eine Zufallszahl und gibt sie Base64 codiert zurück.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandom(int length = 128)
        {
            // Salt erzeugen.
            byte[] salt = new byte[length / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rnd =
                System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Berechnet den HMACSHA256 Wert des Passwortes mit dem übergebenen Salt.
        /// </summary>
        /// <param name="password">Base64 Codiertes Passwort.</param>
        /// <param name="salt">Base64 Codiertes Salt.</param>
        /// <returns></returns>
        protected static string CalculateHash(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException("Invalid Salt or Passwort.");
            }
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash =
                new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird als Hexstring zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
    }
}