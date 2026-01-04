using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SallesController : Controller
{
    private readonly CoworkingDbContext _context;

    public SallesController(CoworkingDbContext context)
    {
        _context = context;
    }

    // GET: /Salles
    public async Task<IActionResult> Index()
    {
        var salles = await _context.Salles.ToListAsync();
        return View(salles);
    }
    // GET: /Salles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Salles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Salle salle)
    {
        if (ModelState.IsValid)
        {
            _context.Add(salle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(salle);
    }

    // GET: /Salles/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salle = await _context.Salles.FindAsync(id);
        if (salle == null)
        {
            return NotFound();
        }

        return View(salle);
    }

    // POST: /Salles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Salle salle)
    {
        if (id != salle.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(salle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalleExists(salle.Id))
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

        return View(salle);
    }

    // GET: /Salles/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salle = await _context.Salles
            .FirstOrDefaultAsync(m => m.Id == id);

        if (salle == null)
        {
            return NotFound();
        }

        return View(salle);
    }

    // POST: /Salles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var salle = await _context.Salles.FindAsync(id);
        if (salle != null)
        {
            _context.Salles.Remove(salle);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Salles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var salle = await _context.Salles
            .Include(s => s.Reservations)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (salle == null)
        {
            return NotFound();
        }

        return View(salle);
    }

    private bool SalleExists(int id)
    {
        return _context.Salles.Any(e => e.Id == id);
    }

}
