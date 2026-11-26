using GePro.DataAccess;

namespace GePro.API.Security
{
    /// <summary>Implementa <see cref="ICurrentUser"/> con el usuario del token JWT de la petición.</summary>
    public class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        public string? UserName
        {
            get
            {
                var identity = httpContextAccessor.HttpContext?.User.Identity;
                if (identity is null || !identity.IsAuthenticated)
                {
                    return null;
                }

                return identity.Name;
            }
        }
    }
}
