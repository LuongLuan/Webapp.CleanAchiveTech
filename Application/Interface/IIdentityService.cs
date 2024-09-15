using Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IIdentityService<T>
    {
        Task<string?> GetUserNameAsync(Guid userId);
        Task<T> GetUserAsync(Guid userId);
        Task<T> GetUserAsync(ClaimsPrincipal user);
        Task<T> GetUserByEmailAsync(string email);

        Task<bool> IsInRoleAsync(Guid userId, string role);

        Task<bool> AuthorizeAsync(Guid userId, string policyName);
        Task<bool> AuthenticationAsync(T user, string password);
        Task<IList<string>> GetRoleUserAsync(T user);
        Task<Result> CreateUserAsync(string userName, string password);
        Task<Result> UpdateUserAsync(T user);
        Task<Result> DeleteUserAsync(Guid userId);
    }
}
