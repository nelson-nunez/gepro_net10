using System;


namespace GePro.Model.Base
{
    public interface ISoftDelete
    {
        public DateTime? Deleted { get; set; }
    }
}
