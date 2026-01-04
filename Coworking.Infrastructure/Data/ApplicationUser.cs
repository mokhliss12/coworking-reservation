using Microsoft.AspNetCore.Identity;

namespace Coworking.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Entreprise { get; set; }
    public string? TypeUtilisateur { get; set; } // Etudiant, Freelance, Startup, Entreprise, Formateur
    public DateTime DateInscription { get; set; } = DateTime.Now;
}

