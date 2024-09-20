
using Domain.Entities;
using Infrastructure.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.IdentityExtension
{
    public static class ClaimsPrincipalExtension
    {
        public static async Task<ApplicationUser?> ToAppUser(this ClaimsPrincipal principal, Func<Guid, Task<ApplicationUser?>> findByIdAsync)
        {
            var claimsPrincipalUser = principal.GetClaimsPrincipalUser();
            ApplicationUser? user = await findByIdAsync(claimsPrincipalUser.Id);

            return user;
        }
        public static async Task<ApplicationUser?> ToAppUser(this ClaimsPrincipal principal, Func<string, Task<ApplicationUser?>> findByIdAsync)
        {
            var claimsPrincipalUser = principal.GetClaimsPrincipalUser();
            ApplicationUser? user = await findByIdAsync(claimsPrincipalUser.Id.ToString());

            return user;
        }
        public static ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token, JwtTokenConfigDto jwtTokenConfig)
        {
            try
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.SecretKey));
                var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false, // expired - no check
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidAudience = jwtTokenConfig.Audience,
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
        public static ClaimsPrincipalUserDto GetClaimsPrincipalUser(this ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;

                ClaimsPrincipalUserDto claimsPrincipalUser = new();
                foreach (var claim in userClaims)
                {
                    switch (claim.Type)
                    {
                        case ClaimTypes.PrimarySid:
                            claimsPrincipalUser.Id = !string.IsNullOrEmpty(claim.Value) ? new Guid(claim.Value) : new Guid();
                            break;
                        case ClaimTypes.Email:
                            claimsPrincipalUser.Email = claim.Value;
                            break;
                        case ClaimTypes.Name:
                        case ClaimTypes.NameIdentifier:
                            claimsPrincipalUser.UserName = claim.Value;
                            break;
                        default:
                            break;
                    }
                }
                claimsPrincipalUser.Roles = userClaims.Where(o => o.Type == ClaimTypes.Role && !string.IsNullOrEmpty(o.Type)).Select(o => o.Type == ClaimTypes.Role ? o.Value : "").ToList();

                return claimsPrincipalUser;
            }

            throw new UnauthorizedAccessException();
        }
        public static JwtSecurityToken CreateJwtSecurityToken(List<Claim> authClaims, JwtTokenConfigDto jwtTokenConfig)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.SecretKey));
            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new(
                            issuer: jwtTokenConfig.Issuer,
                            audience: jwtTokenConfig.Audience,
                            expires: DateTime.Now.AddDays(int.Parse(jwtTokenConfig.TokenValidityInDays)),
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
    }
}
