using Coworking.Core.Entities;
using Coworking.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coworking.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly CoworkingDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationsController(CoworkingDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Notifications
    public async Task<IActionResult> Index(string? search, string? type, bool? estLue)
    {
        var user = await _userManager.GetUserAsync(User);
        IQueryable<Notification> query = _context.Notifications
            .Include(n => n.Reservation);

        // Les clients voient seulement leurs notifications, les admins voient tout
        if (!User.IsInRole("Admin"))
        {
            var userEmail = user?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                query = query.Where(n => n.Reservation != null && n.Reservation.ClientEmail == userEmail);
            }
            else
            {
                // Si l'utilisateur n'a pas d'email, retourner une liste vide
                query = query.Where(n => false);
            }
        }

        // Filtres de recherche
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.Titre.Contains(search) || n.Message.Contains(search));
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(n => n.Type == type);
        }

        if (estLue.HasValue)
        {
            query = query.Where(n => n.EstLue == estLue.Value);
        }

        var notifications = await query.OrderByDescending(n => n.DateCreation).ToListAsync();
        
        ViewBag.Search = search;
        ViewBag.Type = type;
        ViewBag.EstLue = estLue;

        return View(notifications);
    }

    // GET: /Notifications/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var notification = await _context.Notifications
            .Include(n => n.Reservation)
            .ThenInclude(r => r.Salle)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (notification == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent voir que leurs propres notifications
        if (!User.IsInRole("Admin") && (notification.Reservation == null || notification.Reservation.ClientEmail != user?.Email))
        {
            return Forbid();
        }

        // Marquer comme lue
        if (!notification.EstLue)
        {
            notification.EstLue = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
        }

        return View(notification);
    }

    // POST: /Notifications/MarquerLue/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarquerLue(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var notification = await _context.Notifications
            .Include(n => n.Reservation)
            .FirstOrDefaultAsync(n => n.Id == id);
            
        if (notification != null)
        {
            // Les clients ne peuvent marquer que leurs propres notifications
            if (!User.IsInRole("Admin") && (notification.Reservation == null || notification.Reservation.ClientEmail != user!.Email))
            {
                return Forbid();
            }

            notification.EstLue = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Notifications/MarquerToutesLues
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarquerToutesLues()
    {
        var user = await _userManager.GetUserAsync(User);
        IQueryable<Notification> query = _context.Notifications
            .Include(n => n.Reservation)
            .Where(n => !n.EstLue);

        // Les clients ne peuvent marquer que leurs propres notifications
        if (!User.IsInRole("Admin"))
        {
            var userEmail = user?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                query = query.Where(n => n.Reservation != null && n.Reservation.ClientEmail == userEmail);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        var notifications = await query.ToListAsync();

        foreach (var notification in notifications)
        {
            notification.EstLue = true;
        }

        if (notifications.Any())
        {
            _context.UpdateRange(notifications);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Notifications/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var notification = await _context.Notifications
            .Include(n => n.Reservation)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (notification == null)
        {
            return NotFound();
        }

        // Les clients ne peuvent supprimer que leurs propres notifications
        if (!User.IsInRole("Admin") && (notification.Reservation == null || notification.Reservation.ClientEmail != user!.Email))
        {
            return Forbid();
        }

        return View(notification);
    }

    // POST: /Notifications/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var notification = await _context.Notifications
            .Include(n => n.Reservation)
            .FirstOrDefaultAsync(n => n.Id == id);
            
        if (notification != null)
        {
            // Les clients ne peuvent supprimer que leurs propres notifications
            if (!User.IsInRole("Admin") && (notification.Reservation == null || notification.Reservation.ClientEmail != user!.Email))
            {
                return Forbid();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}

