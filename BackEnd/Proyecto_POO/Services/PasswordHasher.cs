using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_POO.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            MemorySize = 1000,
            Iterations = 4

        };

        byte[] hash = argon2.GetBytes(32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string hash, string password)
    {
        var parts = hash.Split(':');
        if (parts.Length != 2) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            MemorySize = 1000,
            Iterations = 4
        };

        byte[] hashToCompare = argon2.GetBytes(32);
        return Convert.ToBase64String(hashToCompare) == parts[1];
    }
}
