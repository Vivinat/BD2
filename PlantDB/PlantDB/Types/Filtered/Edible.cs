namespace PlantDB.Types;

public class Edible
{
    public bool EdibleFruit { get; private set; }
    public bool EdibleLeaves { get; private set; }
    public bool Medicinal { get; private set; }
    public bool PoisonousToHumans { get; private set; }
    public bool PoisonousToPets { get; private set; }

    public Edible(bool edibleFruit, bool edibleLeaves, bool medicinal, bool poisonousToHumans, bool poisonousToPets)
    {
        EdibleFruit = edibleFruit;
        EdibleLeaves = edibleLeaves;
        Medicinal = medicinal;
        PoisonousToHumans = poisonousToHumans;
        PoisonousToPets = poisonousToPets;
    }
}