using AutoMapper;
using GePro.DataAccess;
using GePro.Model.DTO;
using GePro.Model.Exceptions;

namespace GePro.Business.Services
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDTO>> SearchAsync(string? personName, CancellationToken ct);
        Task<CustomerDTO> GetByIdAsync(int id, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }

    public class CustomerService(IUnitOfWork uow, IMapper mapper) : ICustomerService
    {
        public async Task<IReadOnlyList<CustomerDTO>> SearchAsync(string? personName, CancellationToken ct)
        {
            var customers = await uow.Customers.SearchAsync(personName, ct);
            return mapper.Map<IReadOnlyList<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetByIdAsync(int id, CancellationToken ct)
        {
            var customer = await uow.Customers.GetDetailAsync(id, ct)
                ?? throw new NotFoundException($"No existe el cliente {id}.");
            return mapper.Map<CustomerDTO>(customer);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var customer = await uow.Customers.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"No existe el cliente {id}.");

            uow.Customers.Remove(customer);
            await uow.SaveChangesAsync(ct);
        }
    }
}
