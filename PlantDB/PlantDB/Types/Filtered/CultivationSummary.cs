using System.ComponentModel.DataAnnotations;

namespace PlantDB.Types.Filtered;

public class CultivationSummary
{
    [Key] public int id_cultivation { get; set; }
    public string watering { get; set; }
    public string sunlight { get; set; }
    public string scientific_name { get; set; }
}