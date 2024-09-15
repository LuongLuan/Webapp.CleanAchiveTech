using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset RefreshTokenExpiryTime {  get; set; }
    }
}
