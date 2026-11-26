using GePro.DataAccess;
using GePro.Model.DTO;
using GePro.Model.Exceptions;
using GePro.Model.Model;

namespace GePro.Business.Services
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDTO>> SearchAsync(string? personName, CancellationToken ct);
        Task<CustomerDTO> GetByIdAsync(int id, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }

    public class CustomerService(IUnitOfWork uow) : ICustomerService
    {
        public async Task<IReadOnlyList<CustomerDTO>> SearchAsync(string? personName, CancellationToken ct)
        {
            var customers = await uow.Customers.SearchAsync(personName, ct);
            return customers.Select(ToDto).ToList();
        }

        public async Task<CustomerDTO> GetByIdAsync(int id, CancellationToken ct)
        {
            var customer = await uow.Customers.GetDetailAsync(id, ct)
                ?? throw new NotFoundException($"No existe el cliente {id}.");
            return ToDto(customer);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var customer = await uow.Customers.GetByIdAsync(id, ct)
                ?? throw new NotFoundException($"No existe el cliente {id}.");

            uow.Customers.Remove(customer);
            await uow.SaveChangesAsync(ct);
        }

        private static CustomerDTO ToDto(Customer c) => new()
        {
            Id = c.Id,
            Created = c.Created,
            Updated = c.Updated,
            Deleted = c.Deleted,
            PersonId = c.PersonId,
            Code = c.Code,
            CompanyName = c.CompanyName,
            TaxId = c.TaxId,
            Notes = c.Notes,
            PersonName = c.Person.Name,
            PersonLastName = c.Person.LastName,
            PersonDNI = c.Person.DNI,
            PersonBirthDate = c.Person.BirthDate,
            PersonPhone = c.Person.Phone,
            PersonEmail = c.Person.Email,
            PersonAge = c.Person.Age,
            GenderId = c.Person.GenderId,
            AddressId = c.Person.AddressId,
            ProvinceId = c.Person.Address.ProvinceId,
            CityId = c.Person.Address.CityId,
            NeighborhoodId = c.Person.Address.NeighborhoodId,
            NeighborhoodName = c.Person.Address.NeighborhoodName,
            StreetNumber = c.Person.Address.StreetNumber,
        };
    }
}
