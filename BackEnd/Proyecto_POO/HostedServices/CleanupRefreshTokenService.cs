using Microsoft.OpenApi.Writers;
using Proyecto_POO.Data;

namespace Proyecto_POO.HostedServices;

public class CleanupRefreshTokenService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CleanupRefreshTokenService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
            var expiredToken = context.RefreshTokens
                .Where(rt => rt.Expires < DateTime.UtcNow || rt.IsRevoked)
                .ToList();

            context.RefreshTokens.RemoveRange(expiredToken);
            await context.SaveChangesAsync();

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Execute every 24 hours
        }
    }
}
