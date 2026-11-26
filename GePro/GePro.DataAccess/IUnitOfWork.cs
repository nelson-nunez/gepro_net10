using GePro.DataAccess.Repositories;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;
using GePro.Model.ModelUser;

namespace GePro.DataAccess
{
    /// <summary>Único punto que persiste. Una operación de negocio = un SaveChangesAsync.</summary>
    public interface IUnitOfWork
    {
        IRepository<AppUser> AppUsers { get; }
        IRepository<Role> Roles { get; }
        IRepository<Gender> Genders { get; }
        IRepository<Person> Persons { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Employee> Employees { get; }
        IRepository<Province> Provinces { get; }
        IRepository<City> Cities { get; }
        IRepository<Neighborhood> Neighborhoods { get; }
        IRepository<Address> Addresses { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
