using System.Text;
using GePro.API.Security;
using GePro.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GePro.API.Extensions
{
    public static class AuthenticationExtensions
    {
        private const int MinSecretLength = 32;

        /// <summary>JWT Bearer + emisión de tokens + usuario actual para la auditoría.</summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddOptions<JwtTokenConfig>()
                .BindConfiguration(JwtTokenConfig.SectionName)
                .Validate(HasValidSecret, $"{JwtTokenConfig.SectionName}:Secret debe tener al menos {MinSecretLength} caracteres.")
                .Validate(HasIssuerAndAudience, $"{JwtTokenConfig.SectionName}:Issuer y Audience son obligatorios.")
                .ValidateOnStart();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtTokenConfig>>(ConfigureTokenValidation);

            services.AddAuthorization();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, HttpCurrentUser>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }

        private static bool HasValidSecret(JwtTokenConfig config)
        {
            return config.Secret.Length >= MinSecretLength;
        }

        private static bool HasIssuerAndAudience(JwtTokenConfig config)
        {
            return config.Issuer.Length > 0 && config.Audience.Length > 0;
        }

        private static void ConfigureTokenValidation(JwtBearerOptions bearerOptions, IOptions<JwtTokenConfig> jwtOptions)
        {
            var config = jwtOptions.Value;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));

            bearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,
                ValidateAudience = true,
                ValidAudience = config.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                NameClaimType = JwtTokenDefaults.NameClaim,
                RoleClaimType = JwtTokenDefaults.RoleClaim,
            };
        }
    }
}
