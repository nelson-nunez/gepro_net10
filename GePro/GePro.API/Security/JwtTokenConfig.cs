namespace GePro.API.Security
{
    /// <summary>Sección "JwtTokenConfig". El Secret va en user-secrets o variable de entorno, nunca en el repo.</summary>
    public class JwtTokenConfig
    {
        public const string SectionName = "JwtTokenConfig";

        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        /// <summary>Vigencia del access token, en minutos.</summary>
        public int AccessTokenExpiration { get; set; } = 60;
    }
}
