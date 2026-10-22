namespace PragueParking.Data;

public class PriceListData
{
    public Dictionary<string, decimal> PricePerHour { get; set; } = new();
    public int FreeMinutesLimit { get; set; } = 10;
}