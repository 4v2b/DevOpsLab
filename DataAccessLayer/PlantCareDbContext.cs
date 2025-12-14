using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public class PlantCareDbContext : DbContext
{
    public DbSet<Plant> Plants { get; set; }
    public DbSet<Observation> Observations { get; set; }
    public DbSet<CareAction> CareActions { get; set; }
    public DbSet<HealthAssessment> HealthAssessments { get; set; }
    public DbSet<CareActionType> CareActionTypes { get; set; }
    
    public string DbPath { get; }

    public PlantCareDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "plantcare.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
        
        modelBuilder.Entity<Observation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ObservationType).HasMaxLength(50);
            entity.Property(e => e.Source).HasMaxLength(50);
            
            entity.HasOne(e => e.Plant)
                .WithMany(p => p.Observations)
                .HasForeignKey(e => e.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<HealthAssessment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HealthStatus).HasMaxLength(20);
            
            entity.HasOne(e => e.Observation)
                .WithOne(o => o.HealthAssessment)
                .HasForeignKey<HealthAssessment>(e => e.ObservationId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<CareAction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.CareActionType)
                .WithMany(e => e.CareActions)
                .HasForeignKey(e => e.CareActionTypeId);
            entity.Property(e => e.Executor).HasMaxLength(50);
            
            entity.HasOne(e => e.Plant)
                .WithMany(p => p.CareActions)
                .HasForeignKey(e => e.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}