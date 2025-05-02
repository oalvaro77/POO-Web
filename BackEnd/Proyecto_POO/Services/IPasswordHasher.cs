namespace Proyecto_POO.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string passwordH);
        bool VerifyPassword(string hash, string passwordH);

    }
}
