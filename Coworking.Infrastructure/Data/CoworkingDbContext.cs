using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Coworking.Core.Entities;

namespace Coworking.Infrastructure.Data;

public class CoworkingDbContext : IdentityDbContext<ApplicationUser>
{
    public CoworkingDbContext(DbContextOptions<CoworkingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Salle> Salles => Set<Salle>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Facture> Factures => Set<Facture>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Salle>()
            .Property(s => s.PrixHoraire)
            .HasPrecision(10, 2); // ex: 99999999.99

        modelBuilder.Entity<Reservation>()
            .Property(r => r.MontantTotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Facture>()
            .Property(f => f.MontantTotal)
            .HasPrecision(10, 2);
    }
}
