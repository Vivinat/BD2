using PlantDB.Context;
using PlantDB.Types.Filtered;

namespace PlantDB.Controllers;

public class DatabaseInsert(DBContext context)
{
    public void InsertIntoDatabase(
        List<PlantSummary> plantSummaries,
        List<DangerousPlantsSummary> dangerousPlantsSummaries,
        List<PlantDetailsSummary> plantDetailsSummaries, 
        List<CultivationSummary> cultivationSummaries)
    {
        try
        {
            context.AddRange(plantSummaries);
            context.SaveChanges();
            context.AddRange(cultivationSummaries);
            context.SaveChanges();
            context.AddRange(dangerousPlantsSummaries);
            context.SaveChanges();
            context.AddRange(plantDetailsSummaries);
            context.SaveChanges();
            Console.WriteLine($"Inserido {plantSummaries.Count} na tabela plantas, " +
                              $"{dangerousPlantsSummaries.Count} na tabela de plantas perigosas e " +
                              $"{plantDetailsSummaries.Count} na tabela de plant details e " +
                              $"{cultivationSummaries.Count} na tabela de cultivation ");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}