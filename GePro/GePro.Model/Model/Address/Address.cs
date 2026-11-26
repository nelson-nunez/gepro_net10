using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GePro.Model.Base;

namespace GePro.Model.Model.Address
{
    public class Address : BaseEntity
    {
        public Address() { }

        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;

        public int CityId { get; set; }
        public virtual City City { get; set; } = null!;

        public int? NeighborhoodId { get; set; } // Hacer nullable si es opcional
        public virtual Neighborhood? Neighborhood { get; set; } // Hacer nullable si es opcional

        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string? NeighborhoodName { get; set; } // Usar null si NeighborhoodName es opcional

        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string StreetNumber { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{StreetNumber}|{NeighborhoodName ?? Neighborhood?.Name}|{City?.Name}|{Province?.Name}";
        }
    }
}
