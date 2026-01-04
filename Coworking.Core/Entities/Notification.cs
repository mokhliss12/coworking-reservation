namespace Coworking.Core.Entities;

public class Notification
{
    public int Id { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; }

    public bool EstLue { get; set; }

    public string Type { get; set; } = string.Empty; // "Reservation", "Rappel", "Facture"

    // Foreign key
    public int? ReservationId { get; set; }

    // Navigation
    public Reservation? Reservation { get; set; }
}

