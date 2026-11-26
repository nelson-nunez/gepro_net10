using GePro.Business.Services;
using GePro.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GePro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        /// <summary>Lista clientes; <paramref name="name"/> filtra por nombre o apellido.</summary>
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<CustomerDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CustomerDTO>>> Search([FromQuery] string? name, CancellationToken ct)
        {
            var customers = await customerService.SearchAsync(name, ct);
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType<CustomerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> GetById(int id, CancellationToken ct)
        {
            var customer = await customerService.GetByIdAsync(id, ct);
            return Ok(customer);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await customerService.DeleteAsync(id, ct);
            return NoContent();
        }
    }
}
