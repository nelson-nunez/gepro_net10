using GePro.DataAccess;
using GePro.Model.DTO;

namespace GePro.Business.Services
{
    public interface IProvinceService
    {
        Task<IReadOnlyList<ProvinceDTO>> ListAsync(CancellationToken ct);
    }

    public class ProvinceService(IUnitOfWork uow) : IProvinceService
    {
        public async Task<IReadOnlyList<ProvinceDTO>> ListAsync(CancellationToken ct)
        {
            var provinces = await uow.Provinces.ListAsync(null, ct);
            return provinces
                .OrderBy(p => p.Name)
                .Select(p => new ProvinceDTO { Id = p.Id, Name = p.Name })
                .ToList();
        }
    }

    public interface ICityService
    {
        Task<IReadOnlyList<CityDTO>> ListByProvinceAsync(int provinceId, CancellationToken ct);
    }

    public class CityService(IUnitOfWork uow) : ICityService
    {
        public async Task<IReadOnlyList<CityDTO>> ListByProvinceAsync(int provinceId, CancellationToken ct)
        {
            var cities = await uow.Cities.ListByProvinceAsync(provinceId, ct);
            return cities
                .Select(c => new CityDTO { Id = c.Id, Name = c.Name, ProvinceId = c.ProvinceId, ProvinceName = c.Province.Name })
                .ToList();
        }
    }

    public interface INeighborhoodService
    {
        Task<IReadOnlyList<NeighborhoodDTO>> ListByCityAsync(int cityId, CancellationToken ct);
    }

    public class NeighborhoodService(IUnitOfWork uow) : INeighborhoodService
    {
        public async Task<IReadOnlyList<NeighborhoodDTO>> ListByCityAsync(int cityId, CancellationToken ct)
        {
            var neighborhoods = await uow.Neighborhoods.ListByCityAsync(cityId, ct);
            return neighborhoods
                .Select(n => new NeighborhoodDTO { Id = n.Id, Name = n.Name, CityId = n.CityId, CityName = n.City.Name })
                .ToList();
        }
    }

    public interface IGenderService
    {
        Task<IReadOnlyList<GenderDTO>> ListAsync(CancellationToken ct);
    }

    public class GenderService(IUnitOfWork uow) : IGenderService
    {
        public async Task<IReadOnlyList<GenderDTO>> ListAsync(CancellationToken ct)
        {
            var genders = await uow.Genders.ListAsync(null, ct);
            return genders.Select(g => new GenderDTO { Id = g.Id, Name = g.Name }).ToList();
        }
    }
}
