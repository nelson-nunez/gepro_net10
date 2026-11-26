using GePro.Model.Base;
using System;


namespace GePro.Model.Model.Address
{
    public class Neighborhood : BaseEntity
    {
        public Neighborhood() { }

        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public virtual City City { get; set; } = null!;
    }
}
