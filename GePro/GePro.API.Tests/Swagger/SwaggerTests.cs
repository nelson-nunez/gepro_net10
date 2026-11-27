using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GePro.API.Tests.Swagger;

public class SwaggerTests
{
    // La API exige connection string y secreto JWT al arrancar; en tests se proveen valores ficticios.
    private static readonly Dictionary<string, string> ConfiguracionDePrueba = new()
    {
        ["ConnectionStrings:DefaultConnection"] = "Server=(local);Database=GeProTests;Integrated Security=true;TrustServerCertificate=true",
        ["JwtTokenConfig:Secret"] = "clave-solo-para-tests-0123456789-abcdef",
    };

    private static HttpClient CrearCliente(string entorno) =>
        new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.UseEnvironment(entorno);
                foreach (var (clave, valor) in ConfiguracionDePrueba)
                    b.UseSetting(clave, valor);
            })
            .CreateClient();

    [Fact]
    public async Task GetSwaggerIndex_Development_Devuelve200ConHtml()
    {
        var response = await CrearCliente("Development").GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("swagger", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetSwaggerIndex_Development_UsaDocumentoOpenApi()
    {
        var cliente = CrearCliente("Development");

        var doc = await cliente.GetAsync("/openapi/v1.json");
        var config = await cliente.GetStringAsync("/swagger/index.js");

        Assert.Equal(HttpStatusCode.OK, doc.StatusCode);
        Assert.Contains("/openapi/v1.json", config);
    }

    [Fact]
    public async Task GetOpenApiDocument_Development_DeclaraEsquemaBearerJwt()
    {
        var cliente = CrearCliente("Development");

        var contenido = await cliente.GetStringAsync("/openapi/v1.json");

        using var documento = JsonDocument.Parse(contenido);
        var esquema = documento.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");
        Assert.Equal("http", esquema.GetProperty("type").GetString());
        Assert.Equal("bearer", esquema.GetProperty("scheme").GetString());
        Assert.Equal("JWT", esquema.GetProperty("bearerFormat").GetString());
        Assert.True(documento.RootElement.TryGetProperty("security", out _));
    }

    [Fact]
    public async Task GetSwaggerIndex_Production_Devuelve404()
    {
        var response = await CrearCliente("Production").GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("http")]
    [InlineData("https")]
    public void LaunchSettings_PerfilWeb_AbreSwaggerEnNavegador(string perfil)
    {
        var dir = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(dir, "GePro.API", "Properties", "launchSettings.json")))
            dir = Directory.GetParent(dir)!.FullName;
        using var json = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(dir, "GePro.API", "Properties", "launchSettings.json")));

        var p = json.RootElement.GetProperty("profiles").GetProperty(perfil);

        Assert.True(p.GetProperty("launchBrowser").GetBoolean());
        Assert.Equal("swagger", p.GetProperty("launchUrl").GetString());
    }
}
