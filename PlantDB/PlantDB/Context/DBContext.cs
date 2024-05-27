using Microsoft.EntityFrameworkCore;
using PlantDB.Types.Filtered;


namespace PlantDB.Context;

public class DBContext : DbContext
{
    
    public DbSet<PlantSummary> PlantDBSet { get; set; }
    public DbSet<PlantDetailsSummary> PlantDetailsDBSet { get; set; }
    public DbSet<DangerousPlantsSummary> DangerousPlantsDBSet { get; set; }
    public DbSet<CultivationSummary> CultivationDBSet { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=myhost;Database=mydatabase;Username=myusername;Password=mypassword");
    }
}