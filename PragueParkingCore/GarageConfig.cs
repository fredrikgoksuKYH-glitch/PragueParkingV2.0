namespace PragueParking.Core;

public class GarageConfig
{
    public int TotalSpots { get; set; } = 100;
    public Dictionary<string, int> VehiclesPerSpot { get; set; } = new()
    {
        {"Bil", 1},
        {"MC", 2}
    };
}