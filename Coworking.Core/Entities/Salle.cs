using System.ComponentModel.DataAnnotations;

namespace Coworking.Core.Entities;

public class Salle
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis")]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "La capacité est requise")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacité doit être supérieure à 0")]
    [Display(Name = "Capacité")]
    public int Capacite { get; set; }

    [Required(ErrorMessage = "Le prix horaire est requis")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Le prix horaire doit être supérieur à 0")]
    [Display(Name = "Prix horaire")]
    public decimal PrixHoraire { get; set; }

    // Navigation
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
