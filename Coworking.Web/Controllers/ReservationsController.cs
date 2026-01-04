using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

[Authorize]
public class ReservationsController : Controller
{
    private readonly CoworkingDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReservationsController(CoworkingDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Reservations
    public async Task<IActionResult> Index(string? search, string? statut, int? salleId, DateTime? dateDebut, DateTime? dateFin)
    {
        var user = await _userManager.GetUserAsync(User);
        IQueryable<Reservation> query = _context.Reservations.Include(r => r.Salle);

        // Les clients voient seulement leurs réservations, les admins voient tout
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(r => r.ClientEmail == user!.Email);
        }

        // Filtres de recherche
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(r => r.ClientNom.Contains(search) 
                || r.ClientEmail.Contains(search)
                || r.Salle.Nom.Contains(search));
        }

        if (!string.IsNullOrEmpty(statut))
        {
            query = query.Where(r => r.Statut == statut);
        }

        if (salleId.HasValue)
        {
            query = query.Where(r => r.SalleId == salleId.Value);
        }

        if (dateDebut.HasValue)
        {
            query = query.Where(r => r.DateDebut >= dateDebut.Value);
        }

        if (dateFin.HasValue)
        {
            query = query.Where(r => r.DateFin <= dateFin.Value);
        }

        var reservations = await query.OrderByDescending(r => r.DateDebut).ToListAsync();
        
        ViewBag.Search = search;
        ViewBag.Statut = statut;
        ViewBag.SalleId = salleId;
        ViewBag.DateDebut = dateDebut;
        ViewBag.DateFin = dateFin;
        ViewBag.Salles = await _context.Salles.ToListAsync();

        return View(reservations);
    }

    // GET: /Reservations/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var reservation = await _context.Reservations
            .Include(r => r.Salle)
            .Include(r => r.Factures)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent voir que leurs propres réservations
        if (!User.IsInRole("Admin") && reservation.ClientEmail != user!.Email)
        {
            return Forbid();
        }

        return View(reservation);
    }

    // GET: /Reservations/Create
    public async Task<IActionResult> Create(int? salleId)
    {
        var salles = await _context.Salles.ToListAsync();
        ViewData["SalleId"] = new SelectList(salles, "Id", "Nom", salleId);
        
        if (salleId.HasValue)
        {
            var salle = salles.FirstOrDefault(s => s.Id == salleId.Value);
            if (salle != null)
            {
                ViewBag.SallePreSelectionnee = salle;
            }
        }
        
        return View();
    }

    // POST: /Reservations/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reservation reservation)
    {
        var user = await _userManager.GetUserAsync(User);
        
        // Pour les clients, utiliser leurs informations depuis le compte et ignorer les erreurs de validation pour ces champs
        if (!User.IsInRole("Admin"))
        {
            reservation.ClientEmail = user!.Email!;
            reservation.ClientNom = $"{user.Prenom} {user.Nom}";
            // Supprimer les erreurs de validation pour les champs client car ils sont remplis automatiquement
            ModelState.Remove(nameof(reservation.ClientEmail));
            ModelState.Remove(nameof(reservation.ClientNom));
            ModelState.Remove(nameof(reservation.ClientTelephone));
        }

        if (ModelState.IsValid)
        {

            // Vérifier les conflits de réservation
            var conflit = await _context.Reservations
                .Where(r => r.SalleId == reservation.SalleId
                    && r.Statut != "Annulee"
                    && ((reservation.DateDebut >= r.DateDebut && reservation.DateDebut < r.DateFin)
                        || (reservation.DateFin > r.DateDebut && reservation.DateFin <= r.DateFin)
                        || (reservation.DateDebut <= r.DateDebut && reservation.DateFin >= r.DateFin)))
                .FirstOrDefaultAsync();

            if (conflit != null)
            {
                ModelState.AddModelError("", "Cette salle est déjà réservée pour cette période.");
                ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
                return View(reservation);
            }

            // Vérifier que DateDebut < DateFin
            if (reservation.DateDebut >= reservation.DateFin)
            {
                ModelState.AddModelError("", "La date de fin doit être postérieure à la date de début.");
                ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
                return View(reservation);
            }

            // Calculer le montant total
            var salle = await _context.Salles.FindAsync(reservation.SalleId);
            if (salle != null)
            {
                var duree = (reservation.DateFin - reservation.DateDebut).TotalHours;
                reservation.MontantTotal = salle.PrixHoraire * (decimal)duree;
            }

            reservation.Statut = "EnAttente";

            _context.Add(reservation);
            await _context.SaveChangesAsync();

            // Créer une notification
            var notification = new Notification
            {
                Titre = "Nouvelle réservation",
                Message = $"Réservation créée pour la salle {salle?.Nom} du {reservation.DateDebut:dd/MM/yyyy HH:mm} au {reservation.DateFin:dd/MM/yyyy HH:mm}",
                DateCreation = DateTime.Now,
                EstLue = false,
                Type = "Reservation",
                ReservationId = reservation.Id
            };
            _context.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
        return View(reservation);
    }

    // GET: /Reservations/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent modifier que leurs propres réservations
        if (!User.IsInRole("Admin") && reservation.ClientEmail != user!.Email)
        {
            return Forbid();
        }

        ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
        return View(reservation);
    }

    // POST: /Reservations/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Reservation reservation)
    {
        if (id != reservation.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Vérifier les conflits de réservation (exclure la réservation actuelle)
                var conflit = await _context.Reservations
                    .Where(r => r.Id != reservation.Id
                        && r.SalleId == reservation.SalleId
                        && r.Statut != "Annulee"
                        && ((reservation.DateDebut >= r.DateDebut && reservation.DateDebut < r.DateFin)
                            || (reservation.DateFin > r.DateDebut && reservation.DateFin <= r.DateFin)
                            || (reservation.DateDebut <= r.DateDebut && reservation.DateFin >= r.DateFin)))
                    .FirstOrDefaultAsync();

                if (conflit != null)
                {
                    ModelState.AddModelError("", "Cette salle est déjà réservée pour cette période.");
                    ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
                    return View(reservation);
                }

                // Vérifier que DateDebut < DateFin
                if (reservation.DateDebut >= reservation.DateFin)
                {
                    ModelState.AddModelError("", "La date de fin doit être postérieure à la date de début.");
                    ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
                    return View(reservation);
                }

                // Recalculer le montant total
                var salle = await _context.Salles.FindAsync(reservation.SalleId);
                if (salle != null)
                {
                    var duree = (reservation.DateFin - reservation.DateDebut).TotalHours;
                    reservation.MontantTotal = salle.PrixHoraire * (decimal)duree;
                }

                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["SalleId"] = new SelectList(await _context.Salles.ToListAsync(), "Id", "Nom", reservation.SalleId);
        return View(reservation);
    }

    // GET: /Reservations/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var reservation = await _context.Reservations
            .Include(r => r.Salle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent supprimer que leurs propres réservations
        if (!User.IsInRole("Admin") && reservation.ClientEmail != user!.Email)
        {
            return Forbid();
        }

        return View(reservation);
    }

    // POST: /Reservations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            reservation.Statut = "Annulee";
            _context.Update(reservation);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Reservations/Confirmer/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Confirmer(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Salle)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation != null)
        {
            reservation.Statut = "Confirmee";
            _context.Update(reservation);

            // Créer une notification
            var notification = new Notification
            {
                Titre = "Réservation confirmée",
                Message = $"Votre réservation pour la salle {reservation.Salle.Nom} a été confirmée.",
                DateCreation = DateTime.Now,
                EstLue = false,
                Type = "Reservation",
                ReservationId = reservation.Id
            };
            _context.Add(notification);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ReservationExists(int id)
    {
        return _context.Reservations.Any(e => e.Id == id);
    }
}

