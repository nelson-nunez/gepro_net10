using GePro.DataAccess.Repositories;
using GePro.Model.Base;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;
using GePro.Model.ModelUser;
using Microsoft.EntityFrameworkCore;

namespace GePro.DataAccess
{
    public class UnitOfWork(DbModelContext context, ICurrentUser currentUser, TimeProvider clock) : IUnitOfWork
    {
        public IAppUserRepository AppUsers { get; } = new AppUserRepository(context);
        public IRepository<Role> Roles { get; } = new Repository<Role>(context);
        public IRepository<Gender> Genders { get; } = new Repository<Gender>(context);
        public IRepository<Person> Persons { get; } = new Repository<Person>(context);
        public ICustomerRepository Customers { get; } = new CustomerRepository(context);
        public IRepository<Employee> Employees { get; } = new Repository<Employee>(context);
        public IRepository<Province> Provinces { get; } = new Repository<Province>(context);
        public ICityRepository Cities { get; } = new CityRepository(context);
        public INeighborhoodRepository Neighborhoods { get; } = new NeighborhoodRepository(context);
        public IRepository<Address> Addresses { get; } = new Repository<Address>(context);

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            ApplyAudit();
            return await context.SaveChangesAsync(ct);
        }

        private void ApplyAudit()
        {
            var now = clock.GetUtcNow().UtcDateTime;
            var user = currentUser.UserName ?? string.Empty;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = now;
                        entry.Entity.CreatedBy = user;
                        break;

                    case EntityState.Modified:
                        entry.Entity.Updated = now;
                        entry.Entity.UpdatedBy = user;
                        break;

                    case EntityState.Deleted:
                        // Borrado lógico: BaseEntity implementa ISoftDelete.
                        entry.State = EntityState.Modified;
                        entry.Entity.Deleted = now;
                        entry.Entity.DeletedBy = user;
                        break;
                }
            }
        }
    }
}
