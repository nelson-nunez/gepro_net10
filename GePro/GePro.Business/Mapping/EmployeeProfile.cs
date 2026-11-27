using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.Model;

namespace GePro.Business.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // Mismo aplanado de Person y Address que CustomerProfile.
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(
                    dto => dto.GenderId,
                    options => options.MapFrom(employee => employee.Person.GenderId))
                .ForMember(
                    dto => dto.AddressId,
                    options => options.MapFrom(employee => employee.Person.AddressId))
                .ForMember(
                    dto => dto.ProvinceId,
                    options => options.MapFrom(employee => employee.Person.Address.ProvinceId))
                .ForMember(
                    dto => dto.CityId,
                    options => options.MapFrom(employee => employee.Person.Address.CityId))
                .ForMember(
                    dto => dto.NeighborhoodId,
                    options => options.MapFrom(employee => employee.Person.Address.NeighborhoodId))
                .ForMember(
                    dto => dto.NeighborhoodName,
                    options => options.MapFrom(employee => employee.Person.Address.NeighborhoodName))
                .ForMember(
                    dto => dto.StreetNumber,
                    options => options.MapFrom(employee => employee.Person.Address.StreetNumber));
        }
    }
}
