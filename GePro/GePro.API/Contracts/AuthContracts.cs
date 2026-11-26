using System.ComponentModel.DataAnnotations;

namespace GePro.API.Contracts
{
    public class SigninRequest
    {
        [Required]
        public string UserName { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }

    public record AuthTokenResponse(string AccessToken, DateTime ExpiresAt);
}
