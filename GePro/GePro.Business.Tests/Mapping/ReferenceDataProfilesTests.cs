using GePro.Model.DTO;
using GePro.Model.Model.Address;

namespace GePro.Business.Tests.Mapping
{
    public class ReferenceDataProfilesTests
    {
        private readonly AutoMapper.IMapper _mapper = MapperFactory.CreateMapper();

        [Fact]
        public void Map_Province_CopiaIdNombreYAuditoria()
        {
            var created = new DateTime(2025, 1, 1);
            var province = new Province { Id = 4, Name = "Chaco", Created = created };

            var dto = _mapper.Map<ProvinceDTO>(province);

            Assert.Equal(4, dto.Id);
            Assert.Equal("Chaco", dto.Name);
            Assert.Equal(created, dto.Created);
        }

        [Fact]
        public void Map_CityConProvincia_CompletaProvinceName()
        {
            var province = new Province { Id = 4, Name = "Chaco" };
            var city = new City { Id = 1, Name = "Resistencia", ProvinceId = 4, Province = province };

            var dto = _mapper.Map<CityDTO>(city);

            Assert.Equal(1, dto.Id);
            Assert.Equal("Resistencia", dto.Name);
            Assert.Equal(4, dto.ProvinceId);
            Assert.Equal("Chaco", dto.ProvinceName);
        }

        [Fact]
        public void Map_NeighborhoodConCity_CompletaCityName()
        {
            var city = new City { Id = 1, Name = "Resistencia" };
            var neighborhood = new Neighborhood { Id = 7, Name = "Centro", CityId = 1, City = city };

            var dto = _mapper.Map<NeighborhoodDTO>(neighborhood);

            Assert.Equal(7, dto.Id);
            Assert.Equal("Centro", dto.Name);
            Assert.Equal(1, dto.CityId);
            Assert.Equal("Resistencia", dto.CityName);
        }
    }
}
