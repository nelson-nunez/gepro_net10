using GePro.Business.Security;
using GePro.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GePro.Business.Tests
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _sut = new();

        [Fact]
        public void Hash_ContrasenaValida_NoDevuelveTextoPlano()
        {
            var hash = _sut.Hash("Secreta123");

            Assert.NotEqual("Secreta123", hash);
            Assert.DoesNotContain("Secreta123", hash);
        }

        [Fact]
        public void Hash_MismaContrasenaDosVeces_GeneraHashesDistintos()
        {
            Assert.NotEqual(_sut.Hash("Secreta123"), _sut.Hash("Secreta123"));
        }

        [Fact]
        public void Verify_ContrasenaCorrecta_DevuelveTrue()
        {
            var hash = _sut.Hash("Secreta123");

            Assert.True(_sut.Verify(hash, "Secreta123"));
        }

        [Fact]
        public void Verify_ContrasenaIncorrecta_DevuelveFalse()
        {
            var hash = _sut.Hash("Secreta123");

            Assert.False(_sut.Verify(hash, "otra"));
        }

        [Fact]
        public void Seed_UsuarioAdmin_NoGuardaContrasenaEnClaro()
        {
            var options = new DbContextOptionsBuilder<DbModelContext>()
                .UseSqlServer("Server=x;Database=x").Options;
            using var db = new DbModelContext(options);

            var admin = db.GetService<IDesignTimeModel>().Model
                .FindEntityType(typeof(GePro.Model.ModelUser.AppUser))!
                .GetSeedData().Single();

            var hash = (string)admin["PasswordHash"]!;
            Assert.NotEqual("admin", hash);
            Assert.True(_sut.Verify(hash, "admin"));
        }
    }
}
