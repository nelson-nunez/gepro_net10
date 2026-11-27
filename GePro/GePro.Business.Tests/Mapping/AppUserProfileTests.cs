using GePro.Model.DTO;
using GePro.Model.ModelUser;

namespace GePro.Business.Tests.Mapping
{
    public class AppUserProfileTests
    {
        [Fact]
        public void Map_AppUserConRol_CopiaIdNombreRolYPermisos()
        {
            var mapper = MapperFactory.CreateMapper();
            var user = new AppUser
            {
                Id = 1,
                Name = "admin",
                Role = new Role { Name = "Admin", Permissions = "FullAccess" },
            };

            var dto = mapper.Map<AuthenticatedUserDTO>(user);

            Assert.Equal(1, dto.Id);
            Assert.Equal("admin", dto.Name);
            Assert.Equal("Admin", dto.RoleName);
            Assert.Equal("FullAccess", dto.Permissions);
        }
    }
}
