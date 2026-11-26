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
        IAppUserRepository AppUsers { get; }
        IRepository<Role> Roles { get; }
        IRepository<Gender> Genders { get; }
        IRepository<Person> Persons { get; }
        ICustomerRepository Customers { get; }
        IRepository<Employee> Employees { get; }
        IRepository<Province> Provinces { get; }
        ICityRepository Cities { get; }
        INeighborhoodRepository Neighborhoods { get; }
        IRepository<Address> Addresses { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
