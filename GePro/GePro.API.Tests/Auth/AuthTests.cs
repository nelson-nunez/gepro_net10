using System.Net;
using System.Net.Http.Json;
using System.Text;
using GePro.API.Security;
using GePro.Model.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GePro.API.Tests.Auth;

public class AuthTests
{
    private const string Secret = "clave-solo-para-tests-0123456789-abcdef";

    private static HttpClient CrearCliente() =>
        new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.UseSetting("ConnectionStrings:DefaultConnection", "Server=(local);Database=GeProTests;Integrated Security=true;TrustServerCertificate=true");
                b.UseSetting("JwtTokenConfig:Secret", Secret);
            })
            .CreateClient();

    [Theory]
    [InlineData("/api/provinces")]
    [InlineData("/api/provinces/1/cities")]
    [InlineData("/api/cities/1/neighborhoods")]
    [InlineData("/api/Gender")]
    [InlineData("/api/Customer")]
    public async Task Get_SinToken_Devuelve401(string ruta)
    {
        var response = await CrearCliente().GetAsync(ruta);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authenticate_SinCredenciales_Devuelve400()
    {
        var response = await CrearCliente().PostAsJsonAsync("/api/Authentication/Authenticate", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccessToken_UsuarioValido_EmiteTokenFirmadoConClaims()
    {
        var config = new JwtTokenConfig { Secret = Secret, Issuer = "GePro.API", Audience = "GePro.UI", AccessTokenExpiration = 30 };
        var sut = new JwtTokenService(Options.Create(config), TimeProvider.System);

        var result = sut.CreateAccessToken(new AuthenticatedUserDTO { Id = 7, Name = "admin", RoleName = "Admin" });

        var validation = await new JsonWebTokenHandler().ValidateTokenAsync(result.AccessToken, new TokenValidationParameters
        {
            ValidIssuer = "GePro.API",
            ValidAudience = "GePro.UI",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
        });

        Assert.True(validation.IsValid);
        Assert.Equal("admin", validation.Claims[JwtTokenDefaults.NameClaim]);
        Assert.Equal("Admin", validation.Claims[JwtTokenDefaults.RoleClaim]);
    }
}
