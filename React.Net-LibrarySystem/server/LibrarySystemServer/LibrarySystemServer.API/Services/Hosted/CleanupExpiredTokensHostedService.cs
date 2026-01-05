using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LibrarySystemServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace LibrarySystemServer.Services.Hosted
{
    public class CleanupExpiredTokensHostedService : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromHours(1); // run every hour
        private readonly IServiceProvider _services;
        private readonly ILogger<CleanupExpiredTokensHostedService> _logger;

        public CleanupExpiredTokensHostedService(IServiceProvider services, ILogger<CleanupExpiredTokensHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CleanupExpiredTokensHostedService started. Interval: {Interval}", _interval);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var now = DateTime.UtcNow;

                        using var scope = _services.CreateScope();
                        var _context = scope.ServiceProvider.GetRequiredService<LibrarySystemContext>();

                        // Delete expired revoked tokens
                        var revokedToDelete = await _context.RevokedTokens
                            .Where(rt => rt.ExpiresAt <= now)
                            .ToListAsync(stoppingToken);

                        if (revokedToDelete.Count > 0)
                        {
                            _context.RevokedTokens.RemoveRange(revokedToDelete);
                            _logger.LogInformation("Deleting {Count} expired revoked tokens.", revokedToDelete.Count);
                        }

                        // Delete expired refresh tokens
                        var refreshToDelete = await _context.RefreshTokens
                            .Where(rt => rt.Expires <= now)
                            .ToListAsync(stoppingToken);

                        if (refreshToDelete.Count > 0)
                        {
                            _context.RefreshTokens.RemoveRange(refreshToDelete);
                            _logger.LogInformation("Deleting {Count} expired refresh tokens.", refreshToDelete.Count);
                        }

                        if (revokedToDelete.Count > 0 || refreshToDelete.Count > 0)
                        {
                            await _context.SaveChangesAsync(stoppingToken);
                        }
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // shutting down
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error cleaning expired tokens");
                    }

                    try
                    {
                        await Task.Delay(_interval, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // stop waiting, service is stopping
                    }
                }
            }
            finally
            {
                _logger.LogInformation("CleanupExpiredTokensHostedService is stopping.");
            }
        }
    }
}
