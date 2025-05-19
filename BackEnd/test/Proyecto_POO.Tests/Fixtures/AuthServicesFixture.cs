using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services;
using Proyecto_POO.Services.Interfaces;

namespace Proyecto_POO.Tests.Fixtures;

public class AuthServicesFixture : IDisposable
{
    public Mock<IUserRepository> UserRepositoryMock { get; }
    public Mock<IPasswordHasher> PasswordHasherMock { get; }
    public Mock<IREfreshTokenRepository> RefreshTokenRepositoryMock { get; }
    public AuthService AuthService { get; }
    public Mock<IConfiguration> ConfigurationMock { get; }
    public Mock<IPersonService> PersonServiceMock { get; }
    public Mock<ILogger<AuthService>> LoggerMock { get; }

    public AuthServicesFixture()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
        PasswordHasherMock = new Mock<IPasswordHasher>();
        RefreshTokenRepositoryMock = new Mock<IREfreshTokenRepository>();
        LoggerMock = new Mock<ILogger<AuthService>>();
        ConfigurationMock = new Mock<IConfiguration>();
        PersonServiceMock = new Mock<IPersonService>();
        AuthService = new AuthService(
            UserRepositoryMock.Object, 
            PasswordHasherMock.Object,   
            ConfigurationMock.Object,
            PersonServiceMock.Object,
            LoggerMock.Object,
            RefreshTokenRepositoryMock.Object
            );
    }

    public void Dispose()
    {

    }
}
