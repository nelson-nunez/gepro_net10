using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model;

namespace GePro.Business.Mapping
{
    public class GenderProfile : Profile
    {
        public GenderProfile()
        {
            CreateMap<Gender, GenderDTO>();
        }
    }
}
