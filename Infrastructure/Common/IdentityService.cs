using Application.Interface;
using Application.Model;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Common
{
    public class IdentityService : IIdentityService<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
        }

        public async Task<string?> GetUserNameAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user?.UserName;
        }
        public async Task<ApplicationUser> GetUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user;
        }

        public async Task<Result> CreateUserAsync(string userName, string password)
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

        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null ? await DeleteUserAsync(user) : Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);

            return result;
        }

        public async Task<bool> AuthenticationAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<IList<string>> GetRoleUserAsync(ApplicationUser user)
        {
            var result = await _userManager.GetRolesAsync(user);
            return result;
        }

        public async Task<Result> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result != null ? Result.Success() : Result.Failure(result!.Errors.Select(x=>x.Description).ToList());
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var result = await _userManager.GetUserAsync(user);
            return result;
        }
    }
}
