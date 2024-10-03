using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password,JwtTokenConfigDto jwtTokenConfigDto);
        Task<ResultDto> RefreshTokenAsync(string token, JwtTokenConfigDto jwtTokenConfigDto);
        Task<IList<string>> GetRoleUserAsync(ApplicationUser user);
        Task<ResultDto> UpdateUserAsync(ApplicationUser user);
        Task<ResultDto> CreateUserAsync(string userName, string password);
        Task<ApplicationUser> GetUserAsync(Guid userId);
    }
}
