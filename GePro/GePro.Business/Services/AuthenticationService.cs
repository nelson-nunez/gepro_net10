using AutoMapper;
using GePro.Business.Security;
using GePro.DataAccess;
using GePro.Model.DTO;
using GePro.Model.Exceptions;

namespace GePro.Business.Services
{
    public interface IAuthenticationService
    {
        /// <summary>Valida las credenciales. Lanza <see cref="UnauthorizedException"/> si no son válidas.</summary>
        Task<AuthenticatedUserDTO> AuthenticateAsync(string userName, string password, CancellationToken ct);
    }

    public class AuthenticationService(IUnitOfWork uow, IPasswordService passwords, IMapper mapper) : IAuthenticationService
    {
        public async Task<AuthenticatedUserDTO> AuthenticateAsync(string userName, string password, CancellationToken ct)
        {
            var user = await uow.AppUsers.GetByNameWithRoleAsync(userName, ct);

            // Mismo mensaje para usuario inexistente y contraseña errónea: no revela cuál falló.
            if (user is null || !passwords.Verify(user.PasswordHash, password))
                throw new UnauthorizedException("Credenciales inválidas.");

            return mapper.Map<AuthenticatedUserDTO>(user);
        }
    }
}
