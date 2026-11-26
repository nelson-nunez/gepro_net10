using System;


namespace GePro.Model.BaseDTO
{
    public abstract class BaseEntityDTO<Tid>
    {
        public virtual Tid Id { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }

    }
}
