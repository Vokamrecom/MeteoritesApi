using MeteoritesApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeteoritesApi.Data;

public class MeteoritesDbContext(DbContextOptions<MeteoritesDbContext> options)
    : DbContext(options)
{
    public DbSet<Meteorite> Meteorites => Set<Meteorite>();
    public DbSet<IngestionRun> IngestionRuns => Set<IngestionRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeteoritesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

