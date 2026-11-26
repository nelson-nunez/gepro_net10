using GePro.Model.Base;
using System;


namespace GePro.Model.Model.Address
{
    public class City : BaseEntity
    {
        public City()
        {
        }

        public string Name { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;
    }
}
