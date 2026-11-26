using GePro.Model.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GePro.API.Middleware
{
    /// <summary>Traduce las excepciones de negocio a ProblemDetails. Cualquier otra excepción sigue al 500 genérico.</summary>
    public class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
        {
            int statusCode;
            string title;

            if (exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                title = "Recurso no encontrado";
            }
            else if (exception is UnauthorizedException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                title = "No autenticado";
            }
            else
            {
                return false;
            }

            httpContext.Response.StatusCode = statusCode;

            var context = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = exception.Message,
                },
            };

            return await problemDetailsService.TryWriteAsync(context);
        }
    }
}
