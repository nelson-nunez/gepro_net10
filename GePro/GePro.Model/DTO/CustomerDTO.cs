using GePro.Model.BaseDTO;

namespace GePro.Model.DTO
{
    /// <summary>Id = Id del Customer; PersonId = Id de la Person asociada.</summary>
    public class CustomerDTO : PersonDTO
    {
        public int PersonId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }
        public string? Notes { get; set; }
    }
}
