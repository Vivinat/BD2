using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using PlantDB.Types.Filtered;


namespace PlantDB.Context;

public class DBContext : DbContext  //Contexto do banco. Classe que determina seu comportamento
{
    
    public DbSet<PlantSummary> plant { get; set; }      //Os dados que ele lida e as tabelas
    public DbSet<PlantDetailsSummary> plant_details { get; set; }
    public DbSet<DangerousPlantsSummary> dangerous_plants { get; set; }
    public DbSet<CultivationSummary> cultivation { get; set; }

    static DBContext()          //Construtor do banco, avisa que existem enums no banco
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Carelevel>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Growthrate>();
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  //String de conexão. Utiliza o driver npgsql
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=PlantDB;Username=postgres;Password=admin; IncludeErrorDetail = true");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)  //Ao criar um modelo, avisa que existem enums
    {
        modelBuilder.HasPostgresEnum<Carelevel>();
        modelBuilder.HasPostgresEnum<Growthrate>();
    }

}