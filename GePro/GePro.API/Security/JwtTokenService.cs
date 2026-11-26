using System.Security.Claims;
using System.Text;
using GePro.API.Contracts;
using GePro.Model.DTO;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GePro.API.Security
{
    public interface IJwtTokenService
    {
        AuthTokenResponse CreateAccessToken(AuthenticatedUserDTO user);
    }

    public class JwtTokenService(IOptions<JwtTokenConfig> options, TimeProvider clock) : IJwtTokenService
    {
        private readonly JwtTokenConfig config = options.Value;

        public AuthTokenResponse CreateAccessToken(AuthenticatedUserDTO user)
        {
            var now = clock.GetUtcNow().UtcDateTime;
            var expiresAt = now.AddMinutes(config.AccessTokenExpiration);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtTokenDefaults.NameClaim, user.Name),
                new(JwtTokenDefaults.RoleClaim, user.RoleName),
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = config.Issuer,
                Audience = config.Audience,
                Expires = expiresAt,
                SigningCredentials = CreateSigningCredentials(),
            };

            var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);
            return new AuthTokenResponse(accessToken, expiresAt);
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }

    /// <summary>Nombres de claim usados al emitir y al validar el token.</summary>
    public static class JwtTokenDefaults
    {
        public const string NameClaim = "name";
        public const string RoleClaim = "role";
    }
}
