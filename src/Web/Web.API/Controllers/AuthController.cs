using Application.Interface;
using Application.Model;
using Infrastructure.Common;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.API.Model;
using Web.API.SettingConfiguration;

namespace Web.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService<ApplicationUser> _identityUserService;
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(
            IOptions<JwtTokenConfig> jwtTokenConfig,
            IHttpContextAccessor httpContextAccessor,
            IIdentityService<ApplicationUser> identityUserService)
        {
            _jwtTokenConfig = jwtTokenConfig.Value;
            _httpContextAccessor = httpContextAccessor;
            _identityUserService = identityUserService;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserInputModel loginInput)
        {
            try
            {
                if (!IsMailValid(loginInput.Email))
                    return new JsonResult(new { code = HttpStatusCode.BadRequest });

                ApplicationUser? user = await _identityUserService.GetUserByEmailAsync(loginInput.Email);
                if (user == null || !await _identityUserService.AuthenticationAsync(user, loginInput.Password))
                {
                    return new JsonResult(new { code = HttpStatusCode.Unauthorized });
                }

                List<Claim> authClaims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                };


                IList<string> userRoles = await _identityUserService.GetRoleUserAsync(user);
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var jwtSecurityToken = CreateJwtSecurityToken(authClaims);

                string refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(int.Parse(_jwtTokenConfig.RefreshTokenValidityInDays));
                var updateUser = await _identityUserService.UpdateUserAsync(user);

                if (updateUser.Succeeded)
                {
                    return
                        new JsonResult
                        (new
                            {
                                code = HttpStatusCode.OK,
                                data = new
                                {
                                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                                    ExpiresAt = jwtSecurityToken.ValidTo.ToLocalTime(),
                                    RefreshToken = refreshToken
                                }
                            }
                        );

                }
                else
                {
                    return new JsonResult(new { code = HttpStatusCode.Unauthorized, data = updateUser.Errors });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = HttpStatusCode.Unauthorized });
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel refreshTokenInput)
        {
            string accessToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(accessToken))
            {
                return new JsonResult(new APIResult { Code = HttpStatusCode.Unauthorized });
            }

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return new JsonResult(new APIResult { Code = HttpStatusCode.Unauthorized });
            }

            var userId = principal.GetClaimsPrincipalUser().Id;
            ApplicationUser? user = await _identityUserService.GetUserAsync(userId);
            if (user == null || user.RefreshToken != refreshTokenInput.RefreshToken || user.RefreshTokenExpiryTime<DateTimeOffset.Now)
            {
                return new JsonResult(new APIResult() { Code = HttpStatusCode.Unauthorized });
            }

            var newJwtSecurityToken = CreateJwtSecurityToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            // Depending on the project, it is possible to recreate it or not
            user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(int.Parse(_jwtTokenConfig.RefreshTokenValidityInDays));
            var updateUser = await _identityUserService.UpdateUserAsync(user);

            if (updateUser.Succeeded)
            {
                return
                    new JsonResult
                    (new APIResult
                    {
                        Code = HttpStatusCode.OK,
                        Data = new
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwtSecurityToken),
                            ExpiresAt = newJwtSecurityToken.ValidTo.ToLocalTime(),
                            RefreshToken = newRefreshToken
                        }
                    }
                    );

            }
            else
            {
                return new JsonResult(new { code = HttpStatusCode.Unauthorized, data = updateUser.Errors });
            }

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var u = HttpContext.User;
            ApplicationUser? user = await User.ToAppUser(_identityUserService.GetUserAsync!);
            //ApplicationUser? user = await _identityUserService.GetUserAsync(u);

            if (user == null)
            {
                return new JsonResult(new APIResult() { Code = HttpStatusCode.Unauthorized });
            }

            return new JsonResult(new APIResult() { Code = HttpStatusCode.OK, Data = user });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerInput)
        {
            var userEmail = await _identityUserService.GetUserByEmailAsync(registerInput.Email);
            if (userEmail != null)
            {
                return new JsonResult(new APIResult() 
                {
                    Code = HttpStatusCode.Unauthorized,
                    Data = "Email address already exists"
                });
            }
            if (registerInput.Password != registerInput.ConfirmPassword)
            {
                return new JsonResult(new APIResult()
                {
                    Code = HttpStatusCode.BadRequest,
                    Data = "Invalid confirm password!"
                });
            }
            else
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = registerInput.Email,
                    UserName = registerInput.Email,

                };
                var res = await _identityUserService.CreateUserAsync(registerInput.Email, registerInput.Password);
                if (!res.Succeeded)
                {
                    return new JsonResult(new APIResult() 
                    {
                        Code = HttpStatusCode.OK,
                        Data = res
                    });
                }
                return new JsonResult(new APIResult()
                {
                    Code = HttpStatusCode.BadRequest,
                    Data = res.Errors
                });
            }

        }

        private JwtSecurityToken CreateJwtSecurityToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.SecretKey));
            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new(
                            issuer: _jwtTokenConfig.Issuer,
                            audience: _jwtTokenConfig.Audience,
                            expires: DateTime.Now.AddDays(int.Parse(_jwtTokenConfig.TokenValidityInDays)),
                            claims: authClaims,
                            signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            try
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.SecretKey));
                var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false, // expired - no check
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtTokenConfig.Issuer,
                    ValidAudience = _jwtTokenConfig.Audience,
                    IssuerSigningKey = authSigningKey
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }

        }
        private bool IsMailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
