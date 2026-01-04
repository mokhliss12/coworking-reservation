using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

// Le planning est accessible à tous (authentifiés et non-authentifiés)
public class PlanningController : Controller
{
    private readonly CoworkingDbContext _context;

    public PlanningController(CoworkingDbContext context)
    {
        _context = context;
    }

    // GET: /Planning
    public async Task<IActionResult> Index(int? salleId, int? mois, int? annee)
    {
        var date = DateTime.Now;
        if (mois.HasValue && annee.HasValue)
        {
            date = new DateTime(annee.Value, mois.Value, 1);
        }

        var salles = await _context.Salles.ToListAsync();
        ViewData["SalleId"] = new SelectList(salles, "Id", "Nom", salleId);

        var reservations = await _context.Reservations
            .Include(r => r.Salle)
            .Where(r => r.Statut != "Annulee"
                && r.DateDebut.Year == date.Year
                && r.DateDebut.Month == date.Month)
            .ToListAsync();

        if (salleId.HasValue)
        {
            reservations = reservations.Where(r => r.SalleId == salleId.Value).ToList();
        }

        ViewBag.Date = date;
        ViewBag.Reservations = reservations;
        ViewBag.SalleId = salleId;

        return View();
    }

    // GET: /Planning/Disponibilites
    public async Task<IActionResult> Disponibilites(int salleId, DateTime date)
    {
        var reservations = await _context.Reservations
            .Where(r => r.SalleId == salleId
                && r.Statut != "Annulee"
                && r.DateDebut.Date == date.Date)
            .OrderBy(r => r.DateDebut)
            .ToListAsync();

        var creneaux = new List<object>();
        var heureDebut = date.Date.AddHours(8); // 8h du matin
        var heureFin = date.Date.AddHours(20); // 20h du soir

        for (var heure = heureDebut; heure < heureFin; heure = heure.AddHours(1))
        {
            var creneauFin = heure.AddHours(1);
            var estDisponible = !reservations.Any(r =>
                (heure >= r.DateDebut && heure < r.DateFin) ||
                (creneauFin > r.DateDebut && creneauFin <= r.DateFin) ||
                (heure <= r.DateDebut && creneauFin >= r.DateFin));

            creneaux.Add(new
            {
                Debut = heure,
                Fin = creneauFin,
                Disponible = estDisponible
            });
        }

        return Json(creneaux);
    }
}

