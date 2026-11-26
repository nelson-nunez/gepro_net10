using GePro.Model.BaseDTO;
using System;


namespace GePro.Model.DTO
{
    public class CityDTO : BaseEntityDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
    }

    public class NeighborhoodDTO : BaseEntityDTO<int>
    {
        public string Name { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
    }

    public class ProvinceDTO : BaseEntityDTO<int>
    {
        public string Name { get; set; } = string.Empty;
    }
}

