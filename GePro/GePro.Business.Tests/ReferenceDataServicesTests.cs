using System.Linq.Expressions;
using GePro.Business.Services;
using GePro.Business.Tests.Mapping;
using GePro.DataAccess;
using GePro.DataAccess.Repositories;
using GePro.Model.Base;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;
using GePro.Model.ModelUser;

namespace GePro.Business.Tests
{
    public class ReferenceDataServicesTests
    {
        [Fact]
        public async Task ListAsync_ProvinciasDesordenadas_DevuelveOrdenadasPorNombre()
        {
            var provinces = new List<Province>
            {
                new() { Id = 2, Name = "Córdoba" },
                new() { Id = 1, Name = "Buenos Aires" },
                new() { Id = 4, Name = "Chaco" },
            };
            var service = new ProvinceService(new FakeUnitOfWork(provinces), MapperFactory.CreateMapper());

            var result = await service.ListAsync(CancellationToken.None);

            Assert.Equal(new[] { "Buenos Aires", "Chaco", "Córdoba" }, result.Select(p => p.Name));
            Assert.Equal(new[] { 1, 4, 2 }, result.Select(p => p.Id));
        }

        // Solo Provinces está implementado; el resto no se usa en estos tests.
        private sealed class FakeUnitOfWork(List<Province> provinces) : IUnitOfWork
        {
            public IRepository<Province> Provinces { get; } = new FakeProvinces(provinces);
            public IAppUserRepository AppUsers => throw new NotSupportedException();
            public IRepository<Role> Roles => throw new NotSupportedException();
            public IRepository<Gender> Genders => throw new NotSupportedException();
            public IRepository<Person> Persons => throw new NotSupportedException();
            public ICustomerRepository Customers => throw new NotSupportedException();
            public IRepository<Employee> Employees => throw new NotSupportedException();
            public ICityRepository Cities => throw new NotSupportedException();
            public INeighborhoodRepository Neighborhoods => throw new NotSupportedException();
            public IRepository<Address> Addresses => throw new NotSupportedException();

            public Task<int> SaveChangesAsync(CancellationToken ct)
            {
                return Task.FromResult(0);
            }
        }

        private sealed class FakeProvinces(List<Province> provinces) : IRepository<Province>
        {
            public Task<IReadOnlyList<Province>> ListAsync(Expression<Func<Province, bool>>? filter, CancellationToken ct)
            {
                IReadOnlyList<Province> result = provinces;
                return Task.FromResult(result);
            }

            public Task<Province?> GetByIdAsync(int id, CancellationToken ct) => throw new NotSupportedException();
            public Task<Province?> FirstOrDefaultAsync(Expression<Func<Province, bool>> filter, CancellationToken ct) => throw new NotSupportedException();
            public Task<bool> AnyAsync(Expression<Func<Province, bool>> filter, CancellationToken ct) => throw new NotSupportedException();
            public Task<PagedDataResponse<Province>> GetPagedAsync(PagingSortFilterRequest request, Expression<Func<Province, bool>>? filter, CancellationToken ct) => throw new NotSupportedException();
            public Task AddAsync(Province entity, CancellationToken ct) => throw new NotSupportedException();
            public void Remove(Province entity) => throw new NotSupportedException();
        }
    }
}
