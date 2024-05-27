namespace PlantDB.Types.Filtered;

public class DangerousPlantsSummary
{
    public int Id { get; set; }
    public string Care_Level { get; set; }
    public bool Thorny { get; set; }
    public bool Poisonous_To_Humans { get; set; }
    public bool Poisonous_To_Pets { get; set; }
    public string ScientificName { get; set; }
}