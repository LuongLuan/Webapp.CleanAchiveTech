using Application.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUserRepostitory
    {
        Task<ApplicationUser> GetUserByIdAsync(Guid id);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string Passwotd);
        Task<ResultDto> UpdateUserAsync(ApplicationUser user);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        public Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<List<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user);
        Task<bool> AuthorizeAsync(ClaimsPrincipal user, string policyName);
        string GetCurrentTokenAsync();
        Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password, JwtTokenConfigDto jwtTokenConfig);
        Task<ResultDto> RefreshTokenAsync(string refreshToken, JwtTokenConfigDto jwtTokenConfig);

    }
}
