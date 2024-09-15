namespace Web.API.SettingConfiguration
{
    public class JwtTokenConfig
    {
        public const string EnvSectionName = "Jwt";

        public string SecretKey { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";

        public string TokenValidityInDays { get; set; } = "";
        public string RefreshTokenValidityInDays { get; set; } = "";
    }
}
