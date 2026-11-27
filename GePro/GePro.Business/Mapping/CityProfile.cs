using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model.Address;

namespace GePro.Business.Mapping
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            // ProvinceName se completa por aplanado: City.Province.Name.
            CreateMap<City, CityDTO>();
        }
    }
}
