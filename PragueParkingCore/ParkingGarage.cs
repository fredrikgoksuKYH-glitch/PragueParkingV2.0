namespace PragueParking.Core;

public record CheckOutResult(Vehicle Vehicle, int SpotNr, DateTime CheckOutTime, TimeSpan ParkedDuration, decimal Price);

public class ParkingGarage
{
    public List<ParkingSpot> Spots { get; } = new();
    public GarageConfig Config { get; private set; }
    public PriceList Prices { get; private set; }

    public ParkingGarage(GarageConfig config, PriceList prices)
    {
        Config = config;
        Prices = prices;
        for (int i = 1; i <= config.TotalSpots; i++)
            Spots.Add(new ParkingSpot(i));
    }

    public void SetPriceList(PriceList prices) => Prices = prices;

    private int MaxPerSpot(VehicleType type) =>
        Config.VehiclesPerSpot.TryGetValue(type.ToString(), out var max) ? Math.Max(1, max) : 1;

    public ParkingSpot? FindAvailableSpot(VehicleType type)
    {
        int max = MaxPerSpot(type);
        return Spots
            .OrderBy(s => s.SpotNr)
            .FirstOrDefault(s => s.CanFit(type, max));
    }
    public bool IsFull(VehicleType type) => FindAvailableSpot(type) is null;
    public bool IsRegNrParked(string regNr) =>
        Spots.Any(s => s.ParkedVehicles.Any(v =>
        v.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase)));

    public ParkingSpot? CheckIn(Vehicle vehicle)
    {
        if (IsRegNrParked(vehicle.RegNr))
            throw new InvalidOperationException($"Fordonet {vehicle.RegNr} är redan incheckat");

        var spot = FindAvailableSpot(vehicle.Type);
        spot?.Park(vehicle);
        return spot;
    }
    public (ParkingSpot Spot, Vehicle Vehicle)? FindVehicle(string regNr)
    {
        foreach (var spot in Spots)
        {
            var vehicle = spot.ParkedVehicles.FirstOrDefault(v =>
                v.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
            if (vehicle is not null) return (spot, vehicle);
        }
        return null;
    }
    public CheckOutResult? CheckOut(string regNr, DateTime checkOutTime)
    {
        var found = FindVehicle(regNr);
        if (found is null) return null;

        var (spot, vehicle) = found.Value;
        var duration = checkOutTime - vehicle.CheckInTime;
        var price = vehicle.CalculatePrice(checkOutTime, Prices.FreeMinutesLimit);

        spot.RemoveVehicle(vehicle.RegNr);
        return new CheckOutResult(vehicle, spot.SpotNr, checkOutTime, duration, price);
    }
    public string? MoveVehicle(string regNr, int newSpotNr)
    {
        var found = FindVehicle(regNr);
        if (found is null) return "Fordonet hittas inte i garaget.";

        var (oldSpot, vehicle) = found.Value;
        var newSpot = Spots.FirstOrDefault(s => s.SpotNr == newSpotNr);
        if (newSpot is null) return "Det finns ingen P-Plats med det numret.";
        if (newSpot.SpotNr == oldSpot.SpotNr) return "Fordonet står redan på platsen.";

        int max = MaxPerSpot(vehicle.Type);
        if (!newSpot.CanFit(vehicle.Type, max)) return "Den nya platsen kan inte ta emot fordonet.";

        oldSpot.RemoveVehicle(regNr);
        newSpot.Park(vehicle);
        return null;
    }

    public int TotalSpots => Spots.Count;
    public int OccupiedSpots => Spots.Count(s => !s.IsEmpty);
    public int FreeSpots => TotalSpots - OccupiedSpots;

    public IEnumerable<(ParkingSpot Spot, Vehicle Vehicle)> AllParkedVehicles() =>
        Spots.SelectMany(s => s.ParkedVehicles.Select(v => (Spot: s, Vehicle: v)))
        .OrderBy(x => x.Spot.SpotNr);

    public bool TryResize(int newTotalSpots, out string? error)
    {
        if (newTotalSpots < 1)
        {
            error = "Antal P-Platser måste vara minst 1.";
            return false;
        }

        if (newTotalSpots < Spots.Count)
        {
            var spotsToRemove = Spots.Where(s => s.SpotNr > newTotalSpots).ToList();
            if (spotsToRemove.Any(s => !s.IsEmpty))
            {
                error = "Kan inte minska anatal P-platser : det står en eller fler fordon på platser som skulle tas bort";
                return false;
            }
            foreach (var s in spotsToRemove) Spots.Remove(s);
        }
        else if (newTotalSpots > Spots.Count)
        {
            int nextNr = Spots.Count == 0 ? 1 : Spots.Max(s => s.SpotNr) + 1;
            for (int i = 0; i < newTotalSpots - Spots.Count; i++)
                Spots.Add(new ParkingSpot(nextNr +  i));
        }

        Config.TotalSpots = newTotalSpots;
        error = null;
        return true;
    }
    public void UpdateConfig(GarageConfig newConfig) => Config = newConfig;
}
