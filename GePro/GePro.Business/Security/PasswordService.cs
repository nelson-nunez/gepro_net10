using GePro.Model.ModelUser;
using Microsoft.AspNetCore.Identity;

namespace GePro.Business.Security
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<AppUser> _hasher = new();

        public string Hash(string password) => _hasher.HashPassword(null!, password);

        public bool Verify(string passwordHash, string password) =>
            _hasher.VerifyHashedPassword(null!, passwordHash, password)
                is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
