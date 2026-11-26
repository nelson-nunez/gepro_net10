using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GePro.Model.Base;
using GePro.Model.ModelUser;

namespace GePro.Model.Model
{
    public class Employee : BaseEntity
    {
        public int PersonId { get; set; }
        public virtual Person.Person Person { get; set; } = null!;

        /// <summary>Legajo del empleado. Único.</summary>
        [Column(TypeName = "VARCHAR"), StringLength(32)]
        public string FileNumber { get; set; } = string.Empty;

        /// <summary>CUIL del empleado.</summary>
        [Column(TypeName = "VARCHAR"), StringLength(16)]
        public string? TaxId { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Position { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string? Department { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        /// <summary>Fecha de egreso; null mientras el empleado está activo.</summary>
        [DataType(DataType.Date)]
        public DateTime? TerminationDate { get; set; }

        /// <summary>Usuario de acceso al sistema, si tiene uno.</summary>
        public int? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
