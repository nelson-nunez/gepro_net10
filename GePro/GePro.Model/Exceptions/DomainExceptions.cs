namespace GePro.Model.Exceptions
{
    /// <summary>Base de las excepciones de negocio; la capa API las traduce a ProblemDetails.</summary>
    public abstract class DomainException(string message) : Exception(message);

    /// <summary>El recurso solicitado no existe (404).</summary>
    public class NotFoundException(string message) : DomainException(message);

    /// <summary>Credenciales inválidas o sesión no autenticada (401).</summary>
    public class UnauthorizedException(string message) : DomainException(message);
}
