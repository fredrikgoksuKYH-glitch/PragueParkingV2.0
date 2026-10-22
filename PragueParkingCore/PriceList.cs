namespace PragueParking.Core;

public class PriceList
{
    public Dictionary<string, decimal> PricePerHour { get; set; } = new()
    {
        { "Bil", 20m },
        { "MC", 10m }
    };

    public int FreeMinutesLimit { get; set; } = 10;

    public decimal GetPrice (string vehicleTypeName) =>
        PricePerHour.TryGetValue(vehicleTypeName, out decimal price) ? price : 0;
}