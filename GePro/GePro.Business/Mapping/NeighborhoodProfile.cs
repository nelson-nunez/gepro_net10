using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model.Address;

namespace GePro.Business.Mapping
{
    public class NeighborhoodProfile : Profile
    {
        public NeighborhoodProfile()
        {
            // CityName se completa por aplanado: Neighborhood.City.Name.
            CreateMap<Neighborhood, NeighborhoodDTO>();
        }
    }
}
