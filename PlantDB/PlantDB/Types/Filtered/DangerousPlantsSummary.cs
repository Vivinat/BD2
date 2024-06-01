using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantDB.Types.Filtered;

public enum Carelevel
{
    nothing,
    low,
    medium,
    moderate,
    high
}
public class DangerousPlantsSummary
{
    [Key]public int id_dangerousp { get; set; }
    public Carelevel care_level { get; set; }
    public bool thorny { get; set; }
    public bool poisonous_to_humans { get; set; }
    public bool poisonous_to_pets { get; set; }
    public string scientific_name { get; set; }
}