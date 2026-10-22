namespace PragueParking.Core;

public class ParkingSpot
{
    public int SpotNr { get; set; }
    public List<Vehicle> ParkedVehicles { get; } = new();

    public ParkingSpot(int spotNr) => SpotNr = spotNr;

    public bool IsEmpty => ParkedVehicles.Count == 0;

    public VehicleType? OccupyingType => IsEmpty ? null : ParkedVehicles[0].Type;

    public bool CanFit(VehicleType type, int maxPerSpotForType)
    {
        if (IsEmpty) return true;
        return OccupyingType == type && ParkedVehicles.Count < maxPerSpotForType;
    }

    public void Park(Vehicle vehicle) => ParkedVehicles.Add(vehicle);

    public bool RemoveVehicle(string regNr)
    {
        var vehicle = ParkedVehicles.FirstOrDefault(v =>
            v.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
        if (vehicle is null) return false;
        ParkedVehicles.Remove(vehicle);
        return true;
    }
}