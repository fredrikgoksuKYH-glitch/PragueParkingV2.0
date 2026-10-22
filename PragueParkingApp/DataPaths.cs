namespace PragueParkingApp;

public static class DataPaths
{
    public static string DataFolder => Path.Combine(AppContext.BaseDirectory, "Data");
    public static string ConfigFile => Path.Combine(DataFolder, "config.json");
    public static string VehiclesFile => Path.Combine(DataFolder, "parkedvehicles.json");
    public static string PriceListFile => Path.Combine(DataFolder, "pricelist.txt");
}
