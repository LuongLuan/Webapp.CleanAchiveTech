namespace Infrastructure.DTO
{
    public class JwtTokenConfigDto
    {
        public const string EnvSectionName = "Jwt";

        public string SecretKey { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";

        public string TokenValidityInDays { get; set; } = "";
        public string RefreshTokenValidityInDays { get; set; } = "";
    }
}
