using System;
using GePro.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace GePro.Model.ModelUser
{
    public class Role : BaseEntity
    {
        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "VARCHAR"), StringLength(256)]
        public string Permissions { get; set; } = string.Empty;

        public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }

}
