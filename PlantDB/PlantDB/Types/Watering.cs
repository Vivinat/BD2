namespace PlantDB.Types;

public class Watering
{
    public string WateringRate { get; private set; }
    public int WaterDepthRequired { get; private set; }
    public int WaterVolumeRequired { get; private set; }
    public string WateringPeriod { get; private set; }

    public Watering(string wateringRate, int waterDepthRequired, int waterVolumeRequired, string wateringPeriod)
    {
        WateringRate = wateringRate;
        WaterDepthRequired = waterDepthRequired;
        WaterVolumeRequired = waterVolumeRequired;
        WateringPeriod = wateringPeriod;
    }
}