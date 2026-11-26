using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GePro.Model.Base;

namespace GePro.Model.ModelUser
{
    public class AppUser : BaseEntity
    {
        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}
