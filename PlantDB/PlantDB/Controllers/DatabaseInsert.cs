using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlantDB.Types.Filtered;

namespace PlantDB.Controllers;

public class DatabaseInsert
{
    private readonly DbContext _context;
    
    public DatabaseInsert(DbContext context)
    {
        _context = context;
    }

    public void InsertIntoDatabase(
        List<PlantSummary> plantSummaries,
        List<DangerousPlantsSummary> dangerousPlantsSummaries,
        List<PlantDetailsSummary> plantDetailsSummaries, 
        List<CultivationSummary> cultivationSummaries)
    {
        _context.AddRange(plantSummaries);
        _context.AddRange(dangerousPlantsSummaries);
        _context.AddRange(plantDetailsSummaries);
        _context.SaveChanges();
    }
}