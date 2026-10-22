using System.Text.Json;

namespace PragueParking.Data;

public class MyFiles
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private static void EnsureDirExists(string filepath)
    {
        var dir = Path.GetDirectoryName(filepath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public bool SaveConfig<T>(string filepath, T config)
    {
        try
        {
            EnsureDirExists(filepath);
            var json = JsonSerializer.Serialize(config, JsonOptions);
            File.WriteAllText(filepath, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public T ReadConfig<T>(string filePath, Func<T> createDefault) where T : class
    {
        if (!File.Exists(filePath))
        {
            var defaultconfig = createDefault();
            SaveConfig(filePath, defaultconfig);
            return defaultconfig;
        }
        try
        {
            var json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<T>(json, JsonOptions);
            return config ?? createDefault();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error JsonException : {ex}");
            return createDefault();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error IOException : {ex}");
            return createDefault();
        }

    }


    public List<VehicleRecord> ReadVehicles(string filepath)
    {
        if (!File.Exists(filepath))
            return new List<VehicleRecord>();

        var json = File.ReadAllText(filepath);
        if (string.IsNullOrWhiteSpace(json)) return new List<VehicleRecord>();

        var records = JsonSerializer.Deserialize<List<VehicleRecord>>(json, JsonOptions);
        return records ?? new List<VehicleRecord>();
    }

    public void SaveVehicles(string filepath, IEnumerable<VehicleRecord> vehicles)
    {
        EnsureDirExists(filepath);
        var json = JsonSerializer.Serialize(vehicles.ToList(), JsonOptions);
        File.WriteAllText(filepath, json);
    }

    public void WriteDefaultPriceListFile(string filepath, PriceListData priceList)
    {
        EnsureDirExists(filepath);
        var lines = new List<string>
        {
        "",
        "Fria Minuter ={priceList.FreeMinutesLimit}",
        "",
        };
        lines.AddRange(priceList.PricePerHour.Select(kv => $"{kv.Key}={kv.Value}"));
        File.WriteAllLines(filepath, lines);
    }

    private static string StripComment(string line)
    {
        var index = line.IndexOf('#');
        return index >= 0 ? line[..index] : line;
    }
    public PriceListData ReadPriceList(string filepath)
    {
        if (!File.Exists(filepath))
        {
            var defaultList = new PriceListData
            {
                PricePerHour = new Dictionary<string, decimal> { { "Bil", 20m }, { "MC", 10m } },
                FreeMinutesLimit = 10
            };
            WriteDefaultPriceListFile(filepath, defaultList);
            return defaultList;
        }

        var priceList = new PriceListData();

        foreach (var rawLine in File.ReadAllLines(filepath))
        {
            var line = StripComment(rawLine).Trim();
            if (line.Length == 0) continue;

            var part = line.Split('=', 2);
            if (part.Length != 2) continue;

            var key = part[0].Trim();
            var value = part[1].Trim();

            if (key.Equals("FreeMinutes", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(value, out var freeMinutes))
                    priceList.FreeMinutesLimit = freeMinutes;
            }
            else
            {
                if (decimal.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var price))
                    priceList.PricePerHour[key] = price;
            }
        }
        return priceList;
    }
}
