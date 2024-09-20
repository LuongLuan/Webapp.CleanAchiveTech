using Application.DTO;
using Azure.Core;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.DTO;
using Infrastructure.IdentityExtension;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ImplementInterface
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultDto> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return result.ToApplicationResult();
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null && await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);

            return result;
        }

        public async Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password, JwtTokenConfigDto jwtTokenConfig)
        {
            var authen = await _userManager.CheckPasswordAsync(user, password);

            if (authen)
            {
                List<Claim> authClaims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                };


                IList<string> userRoles = await GetRoleUserAsync(user);
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var jwtSecurityToken = ClaimsPrincipalExtension.CreateJwtSecurityToken(authClaims, jwtTokenConfig);

                string refreshToken = StringExtension.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(int.Parse(jwtTokenConfig.RefreshTokenValidityInDays));
                var updateUser = await UpdateUserAsync(user);
                if (updateUser != null) return ResultDto.Success(new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresAt = jwtSecurityToken.ValidTo.ToLocalTime(),
                    RefreshToken = refreshToken
                });
            }
            return ResultDto.Failure(new string[] { "Fail UserName or Password!" });
        }

        public async Task<IList<string>> GetRoleUserAsync(ApplicationUser user)
        {
            var result = await _userManager.GetRolesAsync(user);
            return result;
        }

        public async Task<ResultDto> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result != null ? ResultDto.Success(user) : ResultDto.Failure(result!.Errors.Select(x => x.Description).ToList());
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var result = await _userManager.GetUserAsync(user);
            return result;
        }

        public async Task<ApplicationUser> GetUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user;
        }

        public async Task<ResultDto> RefreshTokenAsync(string refreshToken, JwtTokenConfigDto jwtTokenConfig)
        {
            string accessToken = _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken))
            {


                var principal = ClaimsPrincipalExtension.GetPrincipalFromExpiredToken(accessToken, jwtTokenConfig);
                if (principal != null)
                {
                    var userId = principal.GetClaimsPrincipalUser().Id;
                    ApplicationUser? user = await GetUserAsync(userId);
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
