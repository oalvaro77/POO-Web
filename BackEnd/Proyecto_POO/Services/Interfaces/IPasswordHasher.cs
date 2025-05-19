namespace Proyecto_POO.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string passwordH);
        bool VerifyPassword(string hash, string passwordH);

    }
}
