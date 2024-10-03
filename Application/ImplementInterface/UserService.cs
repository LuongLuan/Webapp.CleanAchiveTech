using Application.DTO;
using Application.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.ImplementInterface
{
    public class UserService : IUserService
    {
        private readonly IUserRepostitory _userRepository;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        public UserService(
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IUserRepostitory userRepository)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userRepository = userRepository;
        }

        public async Task<ResultDto> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userRepository.CreateUserAsync(user, password);

            return result.Succeeded
                            ? ResultDto.Success(null)
                            : ResultDto.Failure(result.Errors.Select(e => e.Description));
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userRepository.GetUserByIdAsync( new Guid(userId.ToString()));

            return user != null && await _userRepository.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
        {
            var user = await _userRepository.GetUserByIdAsync(new Guid(userId.ToString()));

            if (user == null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _userRepository.AuthorizeAsync(principal, policyName);

            return result;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var result = await _userRepository.GetUserByEmailAsync(email);

            return result;
        }

        public async Task<ResultDto> AuthenticationAsync(ApplicationUser user, string password, JwtTokenConfigDto jwtTokenConfigDto)
        {
            var authen = await _userRepository.CheckPasswordAsync(user, password);

            if (authen)
            {
                return await _userRepository.AuthenticationAsync(user,password, jwtTokenConfigDto);
            }
            return ResultDto.Failure(new string[] { "Fail UserName or Password!" });
        }

        public async Task<IList<string>> GetRoleUserAsync(ApplicationUser user)
        {
            var result = await _userRepository.GetRolesAsync(user);
            return result;
        }

        public async Task<ResultDto> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userRepository.UpdateUserAsync(user);
            return result != null ? ResultDto.Success(user) : ResultDto.Failure(result!.Errors.Select(x => x).ToList());
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var result = await _userRepository.GetUserAsync(user);
            return result;
        }

        public async Task<ApplicationUser> GetUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(new Guid(userId.ToString()));

            return user;
        }

        public async Task<ResultDto> RefreshTokenAsync(string refreshToken, JwtTokenConfigDto jwtTokenConfigDto)
        {
            var result = await _userRepository.RefreshTokenAsync(refreshToken, jwtTokenConfigDto);
            return result;
        }
    }
}
