namespace GePro.DataAccess
{
    /// <summary>
    /// Usuario autenticado, para la auditoría (CreatedBy/UpdatedBy/DeletedBy).
    /// Lo implementa la capa API, de modo que ni DataAccess ni Business conocen HttpContext.
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>Nombre del usuario autenticado, o null si no hay sesión.</summary>
        string? UserName { get; }
    }
}
