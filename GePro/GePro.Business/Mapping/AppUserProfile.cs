using AutoMapper;
using GePro.Model.DTO;
using GePro.Model.ModelUser;

namespace GePro.Business.Mapping
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            // RoleName se completa por aplanado: AppUser.Role.Name.
            CreateMap<AppUser, AuthenticatedUserDTO>()
                .ForMember(
                    dto => dto.Permissions,
                    options => options.MapFrom(user => user.Role.Permissions));
        }
    }
}
