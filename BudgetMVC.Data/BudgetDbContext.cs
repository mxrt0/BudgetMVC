using BudgetMVC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetMVC.Data;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext()
    {

    }

    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options)
    {

    }

    DbSet<Transaction> Transactions { get; set; }

    DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId);

        base.OnModelCreating(modelBuilder);
    }
}
