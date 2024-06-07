using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantDB.Types.Filtered;

public enum Growthrate
{
    nothing,
    low,
    medium,
    moderate,
    high
}

public class PlantDetailsSummary
{
    [Key]public int id_detail{ get; set; }
    public bool edible_fruit { get; set; }
    public Growthrate growth_rate { get; set; }
    public bool invasive { get; set; }
    public bool indoor { get; set; }
    public string scientific_name{ get; set; }
    public bool medicinal { get; set; }
}