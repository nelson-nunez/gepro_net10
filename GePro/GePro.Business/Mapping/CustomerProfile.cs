using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model;

namespace GePro.Business.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Por aplanado de nombres se completan PersonName, PersonLastName, PersonDNI,
            // PersonBirthDate, PersonPhone, PersonEmail y PersonAge (propiedad calculada de Person).
            CreateMap<Customer, CustomerDTO>()
                .ForMember(
                    dto => dto.GenderId,
                    options => options.MapFrom(customer => customer.Person.GenderId))
                .ForMember(
                    dto => dto.AddressId,
                    options => options.MapFrom(customer => customer.Person.AddressId))
                .ForMember(
                    dto => dto.ProvinceId,
                    options => options.MapFrom(customer => customer.Person.Address.ProvinceId))
                .ForMember(
                    dto => dto.CityId,
                    options => options.MapFrom(customer => customer.Person.Address.CityId))
                .ForMember(
                    dto => dto.NeighborhoodId,
                    options => options.MapFrom(customer => customer.Person.Address.NeighborhoodId))
                .ForMember(
                    dto => dto.NeighborhoodName,
                    options => options.MapFrom(customer => customer.Person.Address.NeighborhoodName))
                .ForMember(
                    dto => dto.StreetNumber,
                    options => options.MapFrom(customer => customer.Person.Address.StreetNumber));
        }
    }
}
