using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Tests.IntegrationTest;

internal class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string passwordH)
    {
        return passwordH;
    }

    public bool VerifyPassword(string hash, string passwordH)
    {
        return hash == passwordH;
    }
}
