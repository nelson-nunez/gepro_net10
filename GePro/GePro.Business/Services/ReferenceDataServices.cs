using AutoMapper;
using GePro.DataAccess;
using GePro.Model.DTO;

namespace GePro.Business.Services
{
    public interface IProvinceService
    {
        Task<IReadOnlyList<ProvinceDTO>> ListAsync(CancellationToken ct);
    }

    public class ProvinceService(IUnitOfWork uow, IMapper mapper) : IProvinceService
    {
        public async Task<IReadOnlyList<ProvinceDTO>> ListAsync(CancellationToken ct)
        {
            var provinces = await uow.Provinces.ListAsync(null, ct);
            var provincesByName = provinces.OrderBy(p => p.Name).ToList();
            return mapper.Map<IReadOnlyList<ProvinceDTO>>(provincesByName);
        }
    }

    public interface ICityService
    {
        Task<IReadOnlyList<CityDTO>> ListByProvinceAsync(int provinceId, CancellationToken ct);
    }

    public class CityService(IUnitOfWork uow, IMapper mapper) : ICityService
    {
        public async Task<IReadOnlyList<CityDTO>> ListByProvinceAsync(int provinceId, CancellationToken ct)
        {
            var cities = await uow.Cities.ListByProvinceAsync(provinceId, ct);
            return mapper.Map<IReadOnlyList<CityDTO>>(cities);
        }
    }

    public interface INeighborhoodService
    {
        Task<IReadOnlyList<NeighborhoodDTO>> ListByCityAsync(int cityId, CancellationToken ct);
    }

    public class NeighborhoodService(IUnitOfWork uow, IMapper mapper) : INeighborhoodService
    {
        public async Task<IReadOnlyList<NeighborhoodDTO>> ListByCityAsync(int cityId, CancellationToken ct)
        {
            var neighborhoods = await uow.Neighborhoods.ListByCityAsync(cityId, ct);
            return mapper.Map<IReadOnlyList<NeighborhoodDTO>>(neighborhoods);
        }
    }

    public interface IGenderService
    {
        Task<IReadOnlyList<GenderDTO>> ListAsync(CancellationToken ct);
    }

    public class GenderService(IUnitOfWork uow, IMapper mapper) : IGenderService
    {
        public async Task<IReadOnlyList<GenderDTO>> ListAsync(CancellationToken ct)
        {
            var genders = await uow.Genders.ListAsync(null, ct);
            return mapper.Map<IReadOnlyList<GenderDTO>>(genders);
        }
    }
}
