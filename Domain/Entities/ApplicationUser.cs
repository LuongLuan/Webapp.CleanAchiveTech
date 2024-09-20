using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset RefreshTokenExpiryTime {  get; set; }
    }
}
