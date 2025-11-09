using Microsoft.EntityFrameworkCore;
using QuickLists.Core.Models;

namespace QuickLists.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContext<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Checklist> Checklists => Set<Checklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Checklist configuration
        modelBuilder.Entity<Checklist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // TODO: Add when implementing authentication
            // entity.Property(e => e.UserId).IsRequired();
            // entity.HasOne(e => e.User)
            //     .WithMany(u => u.Checklists)
            //     .HasForeignKey(e => e.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Relationship with ChecklistItems
            entity.HasMany(e => e.Items)
                .WithOne(i => i.Checklist)
                .HasForeignKey(i => i.ChecklistId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ChecklistItem configuration
        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Checked).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }
}