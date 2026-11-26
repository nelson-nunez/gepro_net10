namespace GePro.Business.Security
{
    public interface IPasswordService
    {
        /// <summary>Devuelve el hash (PBKDF2 con sal aleatoria) de la contraseña.</summary>
        string Hash(string password);

        /// <summary>Indica si la contraseña coincide con el hash almacenado.</summary>
        bool Verify(string passwordHash, string password);
    }
}
