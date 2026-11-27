using GePro.Model.DTO;
using GePro.Model.Model;

namespace GePro.Business.Tests.Mapping
{
    public class GenderProfileTests
    {
        [Fact]
        public void Map_Gender_CopiaIdNombreYAuditoria()
        {
            var mapper = MapperFactory.CreateMapper();
            var created = new DateTime(2025, 1, 1);
            var gender = new Gender { Id = 2, Name = "Femenino", Created = created };

            var dto = mapper.Map<GenderDTO>(gender);

            Assert.Equal(2, dto.Id);
            Assert.Equal("Femenino", dto.Name);
            Assert.Equal(created, dto.Created);
            Assert.Null(dto.Updated);
            Assert.Null(dto.Deleted);
        }
    }
}
