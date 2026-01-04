using Coworking.Infrastructure.Services;

namespace Coworking.Web.BackgroundServices;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Vérifie toutes les heures

    public ReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();
                    await reminderService.SendRemindersAsync();
                }

                _logger.LogInformation("Vérification des rappels effectuée à {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans le service de rappels");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}

