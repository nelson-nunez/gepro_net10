using GePro.Model.BaseDTO;

namespace GePro.Model.DTO
{
    public class PersonDTO : BaseEntityDTO<int>
    {
        //Person
        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonDNI { get; set; } = string.Empty;
        public DateTime PersonBirthDate { get; set; }
        public string PersonPhone { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        /// <summary>Solo lectura: se calcula desde PersonBirthDate.</summary>
        public int PersonAge { get; set; }
        public int GenderId { get; set; }

        //Address
        public int AddressId { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int? NeighborhoodId { get; set; }
        public string? NeighborhoodName { get; set; }
        public string StreetNumber { get; set; } = string.Empty;
    }
}
