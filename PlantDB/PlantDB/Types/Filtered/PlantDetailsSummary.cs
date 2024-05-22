namespace PlantDB.Types.Filtered;

public class PlantDetailsSummary
{
    public int IdDetail{ get; set; }
    public bool Flowers{ get; set; }
    public bool Cones{ get; set; }
    public bool Fruits{ get; set; }
    public bool Edible_Fruit{ get; set; }
    public bool Leaf{ get; set; }
    public bool Medicinal{ get; set; }
    public string Scientific_name{ get; set; }
}