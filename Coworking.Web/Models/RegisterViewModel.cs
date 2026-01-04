using System.ComponentModel.DataAnnotations;

namespace Coworking.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Le nom est requis")]
    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est requis")]
    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Entreprise (optionnel)")]
    public string? Entreprise { get; set; }

    [Required(ErrorMessage = "Le type d'utilisateur est requis")]
    [Display(Name = "Type d'utilisateur")]
    public string TypeUtilisateur { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [StringLength(100, ErrorMessage = "Le mot de passe doit contenir au moins {2} caractères.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Mot de passe")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmer le mot de passe")]
    [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

