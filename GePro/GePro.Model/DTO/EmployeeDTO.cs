using GePro.Model.BaseDTO;

namespace GePro.Model.DTO
{
    /// <summary>Id = Id del Employee; PersonId = Id de la Person asociada.</summary>
    public class EmployeeDTO : PersonDTO
    {
        public int PersonId { get; set; }
        public string FileNumber { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string Position { get; set; } = string.Empty;
        public string? Department { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? AppUserId { get; set; }
    }
}
