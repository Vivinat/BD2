using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using PlantDB.Types.Filtered;


namespace PlantDB.Context;

public class DBContext : DbContext
{
    
    public DbSet<PlantSummary> plant { get; set; }
    public DbSet<PlantDetailsSummary> plant_details { get; set; }
    public DbSet<DangerousPlantsSummary> dangerous_plants { get; set; }
    public DbSet<CultivationSummary> cultivation { get; set; }

    static DBContext()
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Carelevel>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Growthrate>();
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=PlantDB;Username=postgres;Password=admin; IncludeErrorDetail = true");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<Carelevel>();
        modelBuilder.HasPostgresEnum<Growthrate>();
    }

}