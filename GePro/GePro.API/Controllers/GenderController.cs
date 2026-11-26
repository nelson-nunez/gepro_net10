using GePro.Business.Services;
using GePro.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GePro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GenderController(IGenderService genderService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<GenderDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GenderDTO>>> List(CancellationToken ct)
        {
            var genders = await genderService.ListAsync(ct);
            return Ok(genders);
        }
    }
}
