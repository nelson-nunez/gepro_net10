using GePro.Model.BaseDTO;
using System;


namespace GePro.Model.DTO
{
    public class GenderDTO : BaseEntityDTO<int>
    {
        public string Name { get; set; } = string.Empty;

    }
}