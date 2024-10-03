using Application.DTO;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityExtension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ImplementInterface
{
    public class UserRepository : IUserRepostitory
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<List<string>> GetRolesAsync(ApplicationUser user)
        {
            var result = await _userManager.GetRolesAsync(user);
            return result.ToList();
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var result = await _userManager.GetUserAsync(user);
            return result;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(Guid id)
        {
            var result = await _userManager.FindByIdAsync(id.ToString());
            return result;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.IsInRoleAsync(user, role);
            return result;
        }

        public async Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName)
        {
            var result = await _authorizationService.AuthorizeAsync(user, policyName);

            return result.Succeeded;
        }

        public string GetCurrentTokenAsync()
        {
            string result = _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return result;
            
        }
        public async Task<ResultDto> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? ResultDto.Success(null)
                : ResultDto.Failure(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password, JwtTokenConfigDto jwtTokenConfig)
        {
            List<Claim> authClaims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                };


            IList<string> userRoles = await GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var jwtSecurityToken = ClaimsPrincipalExtension.CreateJwtSecurityToken(authClaims, jwtTokenConfig);

            string refreshToken = StringExtension.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(int.Parse(jwtTokenConfig.RefreshTokenValidityInDays));
            var updateUser = await UpdateUserAsync(user);
            if (updateUser != null) 
                return ResultDto.Success(new
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                        ExpiresAt = jwtSecurityToken.ValidTo.ToLocalTime(),
                        RefreshToken = refreshToken
                    });

            return ResultDto.Failure(new string[]  {"Unauthentication!"});
        }

        public async Task<ResultDto> RefreshTokenAsync(string refreshToken, JwtTokenConfigDto jwtTokenConfig)
        {
            string accessToken = _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken))
            {


                var principal = ClaimsPrincipalExtension.GetPrincipalFromExpiredToken(accessToken, jwtTokenConfig);
                if (principal != null)
                {
                    ApplicationUser? user = await GetUserAsync(principal);
                    if (user != null && user.RefreshToken == refreshToken && user.RefreshTokenExpiryTime > DateTimeOffset.Now)
                    {
                        var newJwtSecurityToken = ClaimsPrincipalExtension.CreateJwtSecurityToken(principal.Claims.ToList(), jwtTokenConfig);
                        var newRefreshToken = StringExtension.GenerateRefreshToken();

                        user.RefreshToken = newRefreshToken;
                        // Depending on the project, it is possible to recreate it or not
                        user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(int.Parse(jwtTokenConfig.RefreshTokenValidityInDays));
                        var updateUser = await UpdateUserAsync(user);

                        if (updateUser != null) return ResultDto.Success(new
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwtSecurityToken),
                            ExpiresAt = newJwtSecurityToken.ValidTo.ToLocalTime(),
                            RefreshToken = newRefreshToken
                        });
                    }
                }
            }
            return ResultDto.Failure(new string[] { "Fail authorize!" });
        }
    }
}
