using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Infrastructure.Services;

public class ReminderService
{
    private readonly CoworkingDbContext _context;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(CoworkingDbContext context, ILogger<ReminderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendRemindersAsync()
    {
        try
        {
            // Récupérer les réservations confirmées qui commencent dans les 24 prochaines heures
            var dateLimite = DateTime.Now.AddHours(24);
            var reservations = await _context.Reservations
                .Where(r => r.Statut == "Confirmee"
                    && r.DateDebut > DateTime.Now
                    && r.DateDebut <= dateLimite)
                .Include(r => r.Salle)
                .Include(r => r.Notifications)
                .ToListAsync();

            foreach (var reservation in reservations)
            {
                // Vérifier si un rappel a déjà été envoyé pour cette réservation
                var rappelExistant = reservation.Notifications
                    .Any(n => n.Type == "Rappel" && n.DateCreation.Date == DateTime.Now.Date);

                if (!rappelExistant)
                {
                    var notification = new Notification
                    {
                        Titre = "Rappel de réservation",
                        Message = $"Rappel : Votre réservation pour la salle {reservation.Salle.Nom} commence le {reservation.DateDebut:dd/MM/yyyy à HH:mm}",
                        DateCreation = DateTime.Now,
                        EstLue = false,
                        Type = "Rappel",
                        ReservationId = reservation.Id
                    };

                    _context.Notifications.Add(notification);
                    _logger.LogInformation($"Rappel envoyé pour la réservation {reservation.Id}");
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi des rappels");
        }
    }
}

