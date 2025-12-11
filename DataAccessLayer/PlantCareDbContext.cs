using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public class PlantCareDbContext (DbContextOptions options) : DbContext(options)
{
    public DbSet<Plant> Plants { get; set; }
    public DbSet<Observation> Observations { get; set; }
    public DbSet<CareAction> CareActions { get; set; }
    public DbSet<HealthAssessment> HealthAssessments { get; set; }
}