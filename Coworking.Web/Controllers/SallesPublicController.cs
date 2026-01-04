using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

public class SallesPublicController : Controller
{
    private readonly CoworkingDbContext _context;

    public SallesPublicController(CoworkingDbContext context)
    {
        _context = context;
    }

    // GET: /SallesPublic
    public async Task<IActionResult> Index()
    {
        var salles = await _context.Salles
            .Include(s => s.Reservations.Where(r => r.Statut != "Annulee"))
            .ToListAsync();
        return View(salles);
    }

    // GET: /SallesPublic/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salle = await _context.Salles
            .Include(s => s.Reservations.Where(r => r.Statut != "Annulee"))
            .FirstOrDefaultAsync(m => m.Id == id);

        if (salle == null)
        {
            return NotFound();
        }

        return View(salle);
    }
}

