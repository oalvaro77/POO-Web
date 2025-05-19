using Moq;
using Proyecto_POO.Models;
using Proyecto_POO.Repositories.Interfaces;
using Proyecto_POO.Services;
using Proyecto_POO.Services.Interfaces;
using Proyecto_POO.Tests.Fixtures;

namespace Proyecto_POO.Tests;

public class AuthserviceTest : IClassFixture<AuthServicesFixture>
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly Mock<IREfreshTokenRepository> _freshTokenRepositoryMock;
    private readonly AuthService _authService;
    private readonly AuthServicesFixture _fixture;

    public AuthserviceTest(AuthServicesFixture fixtures)
    {
        _fixture = fixtures;
        _userRepositoryMock = fixtures.UserRepositoryMock;
        _passwordHasher = fixtures.PasswordHasherMock;
        _freshTokenRepositoryMock = fixtures.RefreshTokenRepositoryMock;
        _authService = fixtures.AuthService;
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnTokens()
    {
        //arrange
        var user = new User { Idpersona = 1, Login = "testuser", Password = "hashedpassword", ApiKey = "4112470a75084255984687154d008958" };
        _userRepositoryMock.Setup(x => x.GetUserByLoginAsync("testuser")).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword("hashedpassword", "plaintextpassword")).Returns(true);
        var refreshToken = new RefreshToken { UserId = 1, Token = "generatedtoken", Expires = DateTime.UtcNow.AddDays(7), IsRevoked = false };
        _freshTokenRepositoryMock.Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        // Configure IConfiguration for JWT
        _fixture.ConfigurationMock.Setup(x => x["Jwt:Key"]).Returns("your-very-secure-key-here-256-bits-long");
        _fixture.ConfigurationMock.Setup(x => x["Jwt:Issuer"]).Returns("your-issuer");
        _fixture.ConfigurationMock.Setup(x => x["Jwt:Audience"]).Returns("your-audience");

        //act
        var result = await _authService.Login("testuser", "plaintextpassword", "4112470a75084255984687154d008958");

        //assert
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsNull()
    {
        //arrange
        var user = new User { Idpersona = 1, Login = "testuser", Password = "hashedpassword", ApiKey = "4112470a75084255984687154d008958" };
        _userRepositoryMock.Setup(x => x.GetUserByLoginAsync("testuser")).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword("hashedpassword", "wrongpassword")).Returns(false);

        //act
        var result = await _authService.Login("testuser", "wrongpassword", "4112470a75084255984687154d008958");

        //assert
        Assert.Null(result.AccessToken);
        Assert.Null(result.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidApiKey_ReturnNull()
    {
        //Arrange
        var user = new User { Idpersona = 1, Login = "testuser", Password = "hashedpassword", ApiKey = "4112470a75084255984687154d008958" };
        _userRepositoryMock.Setup(x => x.GetUserByLoginAsync("testuser")).ReturnsAsync(user);

        //Act
        var result = await _authService.Login("testuser", "plaintextpassword", "wrongapikey");

        //Assert
        Assert.Null(result.AccessToken);
        Assert.Null(result.RefreshToken);
    }

    [Fact]
    public async Task Login_DbError_ThrowApplicationException()
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetUserByLoginAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

        //Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationException>(() => _authService.Login("testuser", "plaintextpassword", "4112470a75084255984687154d008958"));
        Assert.Equal("Internal error server", exception.Message);
    }

}
