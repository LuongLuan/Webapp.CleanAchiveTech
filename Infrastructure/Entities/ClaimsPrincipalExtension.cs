using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
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

        public static ClaimsPrincipalUser GetClaimsPrincipalUser(this ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;

                ClaimsPrincipalUser claimsPrincipalUser = new();
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
    }
}
