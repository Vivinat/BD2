namespace PlantDB.Types;

public class Seeds
{
    public int SeedNumber { get; private set; }
    public string Attracts { get; private set; }
    public string Propagation { get; private set; }

    public Seeds(int seedNumber, string attracts, string propagation)
    {
        SeedNumber = seedNumber;
        Attracts = attracts;
        Propagation = propagation;
    }
}