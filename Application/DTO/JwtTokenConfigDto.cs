namespace Application.DTO
{
    public class JwtTokenConfigDto
    {
        public string SecretKey { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";

        public string TokenValidityInDays { get; set; } = "";
        public string RefreshTokenValidityInDays { get; set; } = "";
    }
}
