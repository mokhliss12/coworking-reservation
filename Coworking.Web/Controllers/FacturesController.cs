using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Coworking.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

[Authorize]
public class FacturesController : Controller
{
    private readonly CoworkingDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PdfService _pdfService;

    public FacturesController(
        CoworkingDbContext context, 
        UserManager<ApplicationUser> userManager,
        PdfService pdfService)
    {
        _context = context;
        _userManager = userManager;
        _pdfService = pdfService;
    }

    // GET: /Factures
    public async Task<IActionResult> Index(string? search, bool? estPayee, DateTime? dateDebut, DateTime? dateFin)
    {
        var user = await _userManager.GetUserAsync(User);
        IQueryable<Facture> query = _context.Factures
            .Include(f => f.Reservation)
            .ThenInclude(r => r.Salle);

        // Les clients voient seulement leurs factures, les admins voient tout
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(f => f.Reservation.ClientEmail == user!.Email);
        }

        // Filtres de recherche
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(f => f.Numero.Contains(search)
                || f.Reservation.ClientNom.Contains(search)
                || f.Reservation.ClientEmail.Contains(search)
                || f.Reservation.Salle.Nom.Contains(search));
        }

        if (estPayee.HasValue)
        {
            query = query.Where(f => f.EstPayee == estPayee.Value);
        }

        if (dateDebut.HasValue)
        {
            query = query.Where(f => f.DateEmission >= dateDebut.Value);
        }

        if (dateFin.HasValue)
        {
            query = query.Where(f => f.DateEmission <= dateFin.Value);
        }

        var factures = await query.OrderByDescending(f => f.DateEmission).ToListAsync();
        
        ViewBag.Search = search;
        ViewBag.EstPayee = estPayee;
        ViewBag.DateDebut = dateDebut;
        ViewBag.DateFin = dateFin;

        return View(factures);
    }

    // GET: /Factures/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var facture = await _context.Factures
            .Include(f => f.Reservation)
            .ThenInclude(r => r.Salle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (facture == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent voir que leurs propres factures
        if (!User.IsInRole("Admin") && facture.Reservation.ClientEmail != user!.Email)
        {
            return Forbid();
        }

        return View(facture);
    }

    // GET: /Factures/Create/5 (pour une réservation)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(int? reservationId)
    {
        if (reservationId == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Salle)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        // Vérifier si une facture existe déjà
        var factureExistante = await _context.Factures
            .FirstOrDefaultAsync(f => f.ReservationId == reservationId);

        if (factureExistante != null)
        {
            return RedirectToAction(nameof(Details), new { id = factureExistante.Id });
        }

        var facture = new Facture
        {
            ReservationId = reservation.Id,
            DateEmission = DateTime.Now,
            MontantTotal = reservation.MontantTotal,
            Numero = GenerateNumeroFacture(),
            EstPayee = false
        };

        return View(facture);
    }

    // POST: /Factures/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Facture facture)
    {
        if (ModelState.IsValid)
        {
            // Vérifier si une facture existe déjà
            var factureExistante = await _context.Factures
                .FirstOrDefaultAsync(f => f.ReservationId == facture.ReservationId);

            if (factureExistante != null)
            {
                ModelState.AddModelError("", "Une facture existe déjà pour cette réservation.");
                return View(facture);
            }

            _context.Add(facture);
            await _context.SaveChangesAsync();

            // Créer une notification
            var reservation = await _context.Reservations
                .Include(r => r.Salle)
                .FirstOrDefaultAsync(r => r.Id == facture.ReservationId);

            var notification = new Notification
            {
                Titre = "Nouvelle facture",
                Message = $"Facture #{facture.Numero} générée pour la réservation de la salle {reservation?.Salle.Nom}. Montant: {facture.MontantTotal:F2} MAD",
                DateCreation = DateTime.Now,
                EstLue = false,
                Type = "Facture",
                ReservationId = facture.ReservationId
            };
            _context.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = facture.Id });
        }

        return View(facture);
    }

    // POST: /Factures/MarquerPayee/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarquerPayee(int id)
    {
        var facture = await _context.Factures
            .Include(f => f.Reservation)
            .ThenInclude(r => r.Salle)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (facture != null)
        {
            facture.EstPayee = true;
            _context.Update(facture);

            // Créer une notification
            var notification = new Notification
            {
                Titre = "Facture payée",
                Message = $"La facture #{facture.Numero} a été marquée comme payée.",
                DateCreation = DateTime.Now,
                EstLue = false,
                Type = "Facture",
                ReservationId = facture.ReservationId
            };
            _context.Add(notification);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Factures/DownloadPdf/5
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var facture = await _context.Factures
            .Include(f => f.Reservation)
            .ThenInclude(r => r.Salle)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (facture == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent télécharger que leurs propres factures
        if (!User.IsInRole("Admin") && facture.Reservation.ClientEmail != user!.Email)
        {
            return Forbid();
        }

        var pdfBytes = _pdfService.GenerateFacturePdf(facture);
        return File(pdfBytes, "application/pdf", $"Facture_{facture.Numero}.pdf");
    }

    private string GenerateNumeroFacture()
    {
        var annee = DateTime.Now.Year;
        var dernierNumero = _context.Factures
            .Where(f => f.Numero.StartsWith($"FACT-{annee}-"))
            .OrderByDescending(f => f.Numero)
            .FirstOrDefault()?.Numero;

        if (dernierNumero != null)
        {
            var parts = dernierNumero.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int numero))
            {
                return $"FACT-{annee}-{numero + 1:D4}";
            }
        }

        return $"FACT-{annee}-0001";
    }
}

