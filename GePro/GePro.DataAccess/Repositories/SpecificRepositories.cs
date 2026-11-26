using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.ModelUser;
using Microsoft.EntityFrameworkCore;

namespace GePro.DataAccess.Repositories
{
    public interface ICityRepository : IRepository<City>
    {
        Task<IReadOnlyList<City>> ListByProvinceAsync(int provinceId, CancellationToken ct);
    }

    public class CityRepository(DbModelContext context) : Repository<City>(context), ICityRepository
    {
        public async Task<IReadOnlyList<City>> ListByProvinceAsync(int provinceId, CancellationToken ct) =>
            await DbSet.AsNoTracking()
                .Include(c => c.Province)
                .Where(c => c.ProvinceId == provinceId)
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
    }

    public interface INeighborhoodRepository : IRepository<Neighborhood>
    {
        Task<IReadOnlyList<Neighborhood>> ListByCityAsync(int cityId, CancellationToken ct);
    }

    public class NeighborhoodRepository(DbModelContext context) : Repository<Neighborhood>(context), INeighborhoodRepository
    {
        public async Task<IReadOnlyList<Neighborhood>> ListByCityAsync(int cityId, CancellationToken ct) =>
            await DbSet.AsNoTracking()
                .Include(n => n.City)
                .Where(n => n.CityId == cityId)
                .OrderBy(n => n.Name)
                .ToListAsync(ct);
    }

    public interface ICustomerRepository : IRepository<Customer>
    {
        /// <summary>Clientes con su Person y Address; filtra por nombre o apellido si se indica.</summary>
        Task<IReadOnlyList<Customer>> SearchAsync(string? personName, CancellationToken ct);

        Task<Customer?> GetDetailAsync(int id, CancellationToken ct);
    }

    public class CustomerRepository(DbModelContext context) : Repository<Customer>(context), ICustomerRepository
    {
        public async Task<IReadOnlyList<Customer>> SearchAsync(string? personName, CancellationToken ct)
        {
            var query = WithPerson();
            if (!string.IsNullOrWhiteSpace(personName))
                query = query.Where(c => c.Person.Name.Contains(personName) || c.Person.LastName.Contains(personName));

            return await query.OrderBy(c => c.Person.LastName).ThenBy(c => c.Person.Name).ToListAsync(ct);
        }

        public async Task<Customer?> GetDetailAsync(int id, CancellationToken ct) =>
            await WithPerson().FirstOrDefaultAsync(c => c.Id == id, ct);

        private IQueryable<Customer> WithPerson() =>
            DbSet.AsNoTracking().Include(c => c.Person).ThenInclude(p => p.Address);
    }

    public interface IAppUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> GetByNameWithRoleAsync(string name, CancellationToken ct);
    }

    public class AppUserRepository(DbModelContext context) : Repository<AppUser>(context), IAppUserRepository
    {
        public async Task<AppUser?> GetByNameWithRoleAsync(string name, CancellationToken ct) =>
            await DbSet.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Name == name, ct);
    }
}
