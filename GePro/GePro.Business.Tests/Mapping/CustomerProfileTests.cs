using GePro.Model.DTO;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;

namespace GePro.Business.Tests.Mapping
{
    public class CustomerProfileTests
    {
        private readonly AutoMapper.IMapper _mapper = MapperFactory.CreateMapper();

        [Fact]
        public void Map_CustomerConPersonaYDireccion_CopiaTodosLosCampos()
        {
            var customer = CreateCustomer(neighborhoodId: 9, neighborhoodName: "Villa Libertad");

            var dto = _mapper.Map<CustomerDTO>(customer);

            Assert.Equal(10, dto.Id);
            Assert.Equal(new DateTime(2025, 1, 1), dto.Created);
            Assert.Equal(new DateTime(2025, 2, 2), dto.Updated);
            Assert.Null(dto.Deleted);
            Assert.Equal(20, dto.PersonId);
            Assert.Equal("C-001", dto.Code);
            Assert.Equal("Distribuidora Norte", dto.CompanyName);
            Assert.Equal("30-12345678-9", dto.TaxId);
            Assert.Equal("Cliente frecuente", dto.Notes);
            Assert.Equal("Ana", dto.PersonName);
            Assert.Equal("Pérez", dto.PersonLastName);
            Assert.Equal("30111222", dto.PersonDNI);
            Assert.Equal(new DateTime(1990, 5, 20), dto.PersonBirthDate);
            Assert.Equal("3624000000", dto.PersonPhone);
            Assert.Equal("ana@example.com", dto.PersonEmail);
            Assert.Equal(customer.Person.Age, dto.PersonAge);
            Assert.Equal(2, dto.GenderId);
            Assert.Equal(30, dto.AddressId);
            Assert.Equal(4, dto.ProvinceId);
            Assert.Equal(1, dto.CityId);
            Assert.Equal(9, dto.NeighborhoodId);
            Assert.Equal("Villa Libertad", dto.NeighborhoodName);
            Assert.Equal("Av. Sarmiento 123", dto.StreetNumber);
        }

        [Fact]
        public void Map_CustomerSinBarrio_DejaBarrioNulo()
        {
            var customer = CreateCustomer(neighborhoodId: null, neighborhoodName: null);

            var dto = _mapper.Map<CustomerDTO>(customer);

            Assert.Null(dto.NeighborhoodId);
            Assert.Null(dto.NeighborhoodName);
        }

        private static Customer CreateCustomer(int? neighborhoodId, string? neighborhoodName)
        {
            var address = new Address
            {
                Id = 30,
                ProvinceId = 4,
                CityId = 1,
                NeighborhoodId = neighborhoodId,
                NeighborhoodName = neighborhoodName,
                StreetNumber = "Av. Sarmiento 123",
            };
            var person = new Person
            {
                Id = 20,
                Name = "Ana",
                LastName = "Pérez",
                DNI = "30111222",
                BirthDate = new DateTime(1990, 5, 20),
                Phone = "3624000000",
                Email = "ana@example.com",
                GenderId = 2,
                AddressId = 30,
                Address = address,
            };

            return new Customer
            {
                Id = 10,
                Created = new DateTime(2025, 1, 1),
                Updated = new DateTime(2025, 2, 2),
                PersonId = 20,
                Person = person,
                Code = "C-001",
                CompanyName = "Distribuidora Norte",
                TaxId = "30-12345678-9",
                Notes = "Cliente frecuente",
            };
        }
    }
}
