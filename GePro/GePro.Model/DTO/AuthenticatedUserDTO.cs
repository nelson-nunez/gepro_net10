namespace GePro.Model.DTO
{
    public class AuthenticatedUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Permissions { get; set; } = string.Empty;
    }
}
