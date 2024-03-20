namespace PlantDB.Types;

public class Plant
{
    public int Id { get; private set; }
    public string CommonName { get; private set; }
    public string Origin { get; private set; }
    public string Type { get; private set; }
    public string Cycle { get; private set; }

    public Plant(int id, string commonName, string origin, string type, string cycle)
    {
        Id = id;
        CommonName = commonName;
        Origin = origin;
        Type = type;
        Cycle = cycle;
    }
}