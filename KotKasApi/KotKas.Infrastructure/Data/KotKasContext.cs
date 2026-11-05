using KotKas.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KotKas.Infrastructure.Data;

public class KotKasContext : DbContext
{
    public KotKasContext(DbContextOptions<KotKasContext> options) : base(options)
    {
    }

    public DbSet<Roommate> Roommates => Set<Roommate>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Roommate>(entity =>
        {
            entity.ToTable("Roommate");
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Email).HasMaxLength(150);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("Expense");
            entity.Property(e => e.StoreName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);

            // One-to-many: Roommate (1) - Expense (n)
            entity.HasOne(e => e.Roommate)
                  .WithMany(r => r.Expenses)
                  .HasForeignKey(e => e.RoommateId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Roommate>().HasData(
            new Roommate { Id = 1, Name = "Rémy Guns", Email = "remy@kotkas.be" },
            new Roommate { Id = 2, Name = "Rik Van den Brande", Email = "rik@kotkas.be" },
            new Roommate { Id = 3, Name = "Lotte Peeters", Email = "lotte@kotkas.be" });

        modelBuilder.Entity<Expense>().HasData(
            new Expense { Id = 1, StoreName = "Colruyt", PurchaseDate = new DateTime(2026, 10, 28), TotalAmount = 54.37m, RoommateId = 1 },
            new Expense { Id = 2, StoreName = "Aldi", PurchaseDate = new DateTime(2026, 10, 31), TotalAmount = 23.15m, RoommateId = 2 },
            new Expense { Id = 3, StoreName = "Delhaize", PurchaseDate = new DateTime(2026, 11, 2), TotalAmount = 12.80m, RoommateId = 3 },
            new Expense { Id = 4, StoreName = "Lidl", PurchaseDate = new DateTime(2026, 11, 4), TotalAmount = 31.62m, RoommateId = 1 });
    }
}
