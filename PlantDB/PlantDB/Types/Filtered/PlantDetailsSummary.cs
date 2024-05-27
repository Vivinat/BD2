namespace PlantDB.Types.Filtered;

public class PlantDetailsSummary
{
    public int IdDetail{ get; set; }
    public bool EdibleFruit { get; set; }
    public string Growth_Rate { get; set; }
    public bool Cuisine { get; set; }
    public bool Invasive { get; set; }
    public bool Indoor { get; set; }
    public bool Rare { get; set; }
    public string Rare_Level { get; set; }
    public string Scientific_name{ get; set; }
}