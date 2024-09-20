using Application.DTO;
using Domain.Entities;
using Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password, JwtTokenConfigDto jwtTokenConfig);
        Task<ResultDto> RefreshTokenAsync(string token, JwtTokenConfigDto jwtTokenConfig);
        Task<IList<string>> GetRoleUserAsync(ApplicationUser user);
        Task<ResultDto> UpdateUserAsync(ApplicationUser user);
        Task<ResultDto> CreateUserAsync(string userName, string password);
        Task<ApplicationUser> GetUserAsync(Guid userId);
    }
}
