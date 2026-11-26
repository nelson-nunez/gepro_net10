using System.Linq.Expressions;
using GePro.Business.Security;
using GePro.Business.Services;
using GePro.DataAccess;
using GePro.DataAccess.Repositories;
using GePro.Model.Base;
using GePro.Model.Exceptions;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;
using GePro.Model.ModelUser;

namespace GePro.Business.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly PasswordService _passwords = new();

        private AuthenticationService CreateSut()
        {
            var user = new AppUser
            {
                Id = 1,
                Name = "admin",
                PasswordHash = _passwords.Hash("Secreta123"),
                Role = new Role { Name = "Admin", Permissions = "FullAccess" },
            };
            return new AuthenticationService(new FakeUnitOfWork(user), _passwords);
        }

        [Fact]
        public async Task AuthenticateAsync_CredencialesValidas_DevuelveUsuarioConRol()
        {
            var result = await CreateSut().AuthenticateAsync("admin", "Secreta123", CancellationToken.None);

            Assert.Equal("admin", result.Name);
            Assert.Equal("Admin", result.RoleName);
        }

        [Fact]
        public async Task AuthenticateAsync_ContrasenaIncorrecta_LanzaUnauthorizedException()
        {
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                CreateSut().AuthenticateAsync("admin", "otra", CancellationToken.None));
        }

        [Fact]
        public async Task AuthenticateAsync_UsuarioInexistente_LanzaUnauthorizedException()
        {
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                CreateSut().AuthenticateAsync("nadie", "Secreta123", CancellationToken.None));
        }

        // Solo AppUsers está implementado; el resto no se usa en estos tests.
        private sealed class FakeUnitOfWork(AppUser user) : IUnitOfWork
        {
            public IAppUserRepository AppUsers { get; } = new FakeAppUsers(user);
            public IRepository<Role> Roles => throw new NotSupportedException();
            public IRepository<Gender> Genders => throw new NotSupportedException();
            public IRepository<Person> Persons => throw new NotSupportedException();
            public ICustomerRepository Customers => throw new NotSupportedException();
            public IRepository<Employee> Employees => throw new NotSupportedException();
            public IRepository<Province> Provinces => throw new NotSupportedException();
            public ICityRepository Cities => throw new NotSupportedException();
            public INeighborhoodRepository Neighborhoods => throw new NotSupportedException();
            public IRepository<Address> Addresses => throw new NotSupportedException();
            public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(0);
        }

        private sealed class FakeAppUsers(AppUser user) : IAppUserRepository
        {
            public Task<AppUser?> GetByNameWithRoleAsync(string name, CancellationToken ct) =>
                Task.FromResult(user.Name == name ? user : null);
            public Task<AppUser?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter, CancellationToken ct) => throw new NotSupportedException();

            public Task<AppUser?> GetByIdAsync(int id, CancellationToken ct) => throw new NotSupportedException();
            public Task<bool> AnyAsync(Expression<Func<AppUser, bool>> filter, CancellationToken ct) => throw new NotSupportedException();
            public Task<IReadOnlyList<AppUser>> ListAsync(Expression<Func<AppUser, bool>>? filter, CancellationToken ct) => throw new NotSupportedException();
            public Task<PagedDataResponse<AppUser>> GetPagedAsync(PagingSortFilterRequest request, Expression<Func<AppUser, bool>>? filter, CancellationToken ct) => throw new NotSupportedException();
            public Task AddAsync(AppUser entity, CancellationToken ct) => throw new NotSupportedException();
            public void Remove(AppUser entity) => throw new NotSupportedException();
        }
    }
}
