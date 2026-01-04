namespace Coworking.Core.Entities;

public class Facture
{
    public int Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public DateTime DateEmission { get; set; }

    public decimal MontantTotal { get; set; }

    public bool EstPayee { get; set; }

    // Foreign key
    public int ReservationId { get; set; }

    // Navigation
    public Reservation Reservation { get; set; } = null!;
}

