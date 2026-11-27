using GePro.Model.DTO;
using GePro.Model.Model;
using GePro.Model.Model.Address;
using GePro.Model.Model.Person;

namespace GePro.Business.Tests.Mapping
{
    public class EmployeeProfileTests
    {
        private readonly AutoMapper.IMapper _mapper = MapperFactory.CreateMapper();

        [Fact]
        public void Map_EmployeeConPersonaYDireccion_CopiaTodosLosCampos()
        {
            var employee = CreateEmployee(isTerminated: true);

            var dto = _mapper.Map<EmployeeDTO>(employee);

            Assert.Equal(11, dto.Id);
            Assert.Equal(new DateTime(2025, 1, 1), dto.Created);
            Assert.Equal(21, dto.PersonId);
            Assert.Equal("E-100", dto.FileNumber);
            Assert.Equal("20-11111111-1", dto.TaxId);
            Assert.Equal("Analista", dto.Position);
            Assert.Equal("Sistemas", dto.Department);
            Assert.Equal(new DateTime(2020, 3, 1), dto.HireDate);
            Assert.Equal(new DateTime(2024, 12, 31), dto.TerminationDate);
            Assert.Equal(5, dto.AppUserId);
            Assert.Equal("Luis", dto.PersonName);
            Assert.Equal("Gómez", dto.PersonLastName);
            Assert.Equal("28999888", dto.PersonDNI);
            Assert.Equal(new DateTime(1985, 8, 10), dto.PersonBirthDate);
            Assert.Equal("3624111111", dto.PersonPhone);
            Assert.Equal("luis@example.com", dto.PersonEmail);
            Assert.Equal(employee.Person.Age, dto.PersonAge);
            Assert.Equal(1, dto.GenderId);
            Assert.Equal(31, dto.AddressId);
            Assert.Equal(4, dto.ProvinceId);
            Assert.Equal(1, dto.CityId);
            Assert.Equal(9, dto.NeighborhoodId);
            Assert.Equal("Villa Libertad", dto.NeighborhoodName);
            Assert.Equal("Calle 25 de Mayo 456", dto.StreetNumber);
        }

        [Fact]
        public void Map_EmployeeActivoSinUsuario_DejaOpcionalesNulos()
        {
            var employee = CreateEmployee(isTerminated: false);
            employee.Department = null;
            employee.TaxId = null;
            employee.AppUserId = null;

            var dto = _mapper.Map<EmployeeDTO>(employee);

            Assert.Null(dto.Department);
            Assert.Null(dto.TaxId);
            Assert.Null(dto.TerminationDate);
            Assert.Null(dto.AppUserId);
        }

        private static Employee CreateEmployee(bool isTerminated)
        {
            var address = new Address
            {
                Id = 31,
                ProvinceId = 4,
                CityId = 1,
                NeighborhoodId = 9,
                NeighborhoodName = "Villa Libertad",
                StreetNumber = "Calle 25 de Mayo 456",
            };
            var person = new Person
            {
                Id = 21,
                Name = "Luis",
                LastName = "Gómez",
                DNI = "28999888",
                BirthDate = new DateTime(1985, 8, 10),
                Phone = "3624111111",
                Email = "luis@example.com",
                GenderId = 1,
                AddressId = 31,
                Address = address,
            };

            return new Employee
            {
                Id = 11,
                Created = new DateTime(2025, 1, 1),
                PersonId = 21,
                Person = person,
                FileNumber = "E-100",
                TaxId = "20-11111111-1",
                Position = "Analista",
                Department = "Sistemas",
                HireDate = new DateTime(2020, 3, 1),
                TerminationDate = isTerminated ? new DateTime(2024, 12, 31) : null,
                AppUserId = 5,
            };
        }
    }
}
