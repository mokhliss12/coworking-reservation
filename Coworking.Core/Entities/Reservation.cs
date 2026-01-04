using System.ComponentModel.DataAnnotations;

namespace Coworking.Core.Entities;

public class Reservation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La date de début est requise")]
    [Display(Name = "Date de début")]
    public DateTime DateDebut { get; set; }

    [Required(ErrorMessage = "La date de fin est requise")]
    [Display(Name = "Date de fin")]
    public DateTime DateFin { get; set; }

    [Required(ErrorMessage = "Le nom du client est requis")]
    [Display(Name = "Nom du client")]
    public string ClientNom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email du client est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email du client")]
    public string ClientEmail { get; set; } = string.Empty;

    [Display(Name = "Téléphone du client")]
    public string ClientTelephone { get; set; } = string.Empty;

    public decimal MontantTotal { get; set; }

    public string Statut { get; set; } = "EnAttente"; // "EnAttente", "Confirmee", "Annulee"

    // Foreign key
    [Required(ErrorMessage = "La salle est requise")]
    [Display(Name = "Salle")]
    public int SalleId { get; set; }

    // Navigation
    public Salle Salle { get; set; } = null!;

    public ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
