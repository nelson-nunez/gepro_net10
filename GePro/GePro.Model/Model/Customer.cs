using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GePro.Model.Base;

namespace GePro.Model.Model
{
    public class Customer : BaseEntity
    {
        public int PersonId { get; set; }
        public virtual Person.Person Person { get; set; } = null!;

        /// <summary>Código interno del cliente. Único.</summary>
        [Column(TypeName = "VARCHAR"), StringLength(32)]
        public string Code { get; set; } = string.Empty;

        /// <summary>Razón social, si el cliente es una empresa.</summary>
        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string? CompanyName { get; set; }

        /// <summary>CUIT/CUIL del cliente.</summary>
        [Column(TypeName = "VARCHAR"), StringLength(16)]
        public string? TaxId { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(512)]
        public string? Notes { get; set; }
    }
}
