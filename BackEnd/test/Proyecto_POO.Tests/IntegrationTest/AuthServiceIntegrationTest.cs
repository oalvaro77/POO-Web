using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Proyecto_POO.Data;
using Proyecto_POO.Repositories;
using Proyecto_POO.Services;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Proyecto_POO.Models;

namespace Proyecto_POO.Tests.IntegrationTest;

public class AuthServiceIntegrationTest : IDisposable
{
    private readonly ProjectDbContext _projectDbContext;
    private readonly AuthService authService;
    private readonly IConfiguration _configuration;
    private readonly Mock<IDistributedCache> _distributedCacheMock;

    public AuthServiceIntegrationTest()
    {
        //Configere DB in Memory 
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _projectDbContext = new ProjectDbContext(options);

        //Configure IConfiguration with real values
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "Jwt:Key", "your-very-secure-key-here-256-bits-long" },
                { "Jwt:Issuer", "your-issuer" },
                {  "Jwt:Audience", "your-audience" }
            });
        _configuration = configurationBuilder.Build();

        //Mock IDistributedCache
        _distributedCacheMock = new Mock<IDistributedCache>();
        

        // Inicialite real dependecies
        var userRepository = new UserRepository(_projectDbContext);
        var refreshTokenRepository = new RefreshTokenRepository(_projectDbContext, _distributedCacheMock.Object);
        var passwordHasher = new FakePasswordHasher();
        var personService = new FakePersonService();
        var logger = NullLogger<AuthService>.Instance;

        // Create AuthService with real dependecies
        authService = new AuthService(
            userRepository,
            passwordHasher,
            _configuration,
            personService,
            logger,
            refreshTokenRepository);


        // Insert inital data into the in-memory database
        var user = new User
        {
            Idpersona = 1,
            Login = "testuser",
            Password = "plaintextpassword",
            ApiKey = "4112470a75084255984687154d008958"

        };

        _projectDbContext.Users.Add(user);
        _projectDbContext.SaveChanges();




    }


    [Fact]
    public async Task Login_ValidCredentials_ReturnTokens()
    {
        //Arrange
        // Data is already inserted in the in-memory database in the constructor

        //Act
        var result = await authService.Login("testuser", "plaintextpassword", "4112470a75084255984687154d008958");

        //Assert
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);

        // Verify that the refresh token was saved in  the dadabase
        var refreshToken = await _projectDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == result.RefreshToken);
        Assert.NotNull(refreshToken);
        Assert.Equal(1, refreshToken.UserId);
        Assert.False(refreshToken.IsRevoked);
        Assert.True(refreshToken.Expires > DateTime.UtcNow);

    }
    public void Dispose()
    {
        // Clean up the in-memory database after each test
        _projectDbContext.Database.EnsureDeleted();
        _projectDbContext.Dispose();
    }


}
