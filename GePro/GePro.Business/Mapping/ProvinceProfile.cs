using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model.Address;

namespace GePro.Business.Mapping
{
    public class ProvinceProfile : Profile
    {
        public ProvinceProfile()
        {
            CreateMap<Province, ProvinceDTO>();
        }
    }
}
