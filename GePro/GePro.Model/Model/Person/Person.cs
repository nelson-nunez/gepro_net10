using System;
using GePro.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GePro.Model.Model.Person
{
    public class Person : BaseEntity
    {
        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string LastName { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(16)]
        public string DNI { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(16)]
        public string Phone { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Email { get; set; } = string.Empty;

        public int GenderId { get; set; }
        public virtual Gender Gender { get; set; } = null!;
        
        public int  AddressId { get; set; }
        public virtual Address.Address Address { get; set; } = null!;

        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}

