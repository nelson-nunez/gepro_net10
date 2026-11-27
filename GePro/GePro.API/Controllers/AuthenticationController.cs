using GePro.API.Contracts;
using GePro.API.Security;
using GePro.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GePro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthenticationController(IAuthenticationService authenticationService, IJwtTokenService tokenService) : ControllerBase
    {
        [HttpPost("Authenticate")]
        [ProducesResponseType<AuthTokenResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthTokenResponse>> Authenticate(SigninRequest request, CancellationToken ct)
        {
            var user = await authenticationService.AuthenticateAsync(request.UserName, request.Password, ct);
            var token = tokenService.CreateAccessToken(user);
            return Ok(token);
        }
    }
}
