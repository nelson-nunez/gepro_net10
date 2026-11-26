using GePro.Business.Services;
using GePro.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GePro.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class AddressController(
        IProvinceService provinceService,
        ICityService cityService,
        INeighborhoodService neighborhoodService) : ControllerBase
    {
        [HttpGet("provinces")]
        [ProducesResponseType<IReadOnlyList<ProvinceDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProvinceDTO>>> ListProvinces(CancellationToken ct)
        {
            var provinces = await provinceService.ListAsync(ct);
            return Ok(provinces);
        }

        [HttpGet("provinces/{provinceId:int}/cities")]
        [ProducesResponseType<IReadOnlyList<CityDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CityDTO>>> ListCities(int provinceId, CancellationToken ct)
        {
            var cities = await cityService.ListByProvinceAsync(provinceId, ct);
            return Ok(cities);
        }

        [HttpGet("cities/{cityId:int}/neighborhoods")]
        [ProducesResponseType<IReadOnlyList<NeighborhoodDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NeighborhoodDTO>>> ListNeighborhoods(int cityId, CancellationToken ct)
        {
            var neighborhoods = await neighborhoodService.ListByCityAsync(cityId, ct);
            return Ok(neighborhoods);
        }
    }
}
