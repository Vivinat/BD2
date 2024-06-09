using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantDB.Types.Filtered;

public class PlantSummary
{
    public int id_plant { get; set; }
    public string common_name { get; set; }
    [Key][DatabaseGenerated(DatabaseGeneratedOption.None)]public string scientific_name { get; set; }   //Chave primária
}