using PragueParking.Core;
using PragueParking.Data;

namespace PragueParkingApp;

public class GarageService
{
    private readonly MyFiles _file = new();
    public ParkingGarage Garage { get; private set; }

    private static PriceList MapPriceList(PragueParking.Data.PriceListData data) => new()
    {
        PricePerHour = data.PricePerHour,
        FreeMinutesLimit = data.FreeMinutesLimit
    };
    private void LoadParkedVehicle()
    {
        var records = _file.ReadVehicles(DataPaths.VehiclesFile);
        foreach (var record in records)
        {
            var spot = Garage.Spots.FirstOrDefault(s => s.SpotNr == record.SpotNr);
            if (spot is null) continue;

            Vehicle? vehicle = record.VehicleType switch
            {
                "Bil" => new Car(record.RegNr, record.CheckInTime, record.PricePerHour),
                "MC" => new MC(record.RegNr, record.CheckInTime, record.PricePerHour),
                _ => null
            };
            if (vehicle is not null) spot.Park(vehicle);
        }
    }
    public GarageService()
    {
        var config = _file.ReadConfig(DataPaths.ConfigFile, () => new GarageConfig());
        var prices = MapPriceList(_file.ReadPriceList(DataPaths.PriceListFile));
        Garage = new ParkingGarage(config, prices);
        LoadParkedVehicle();
    }

    public void SaveConfig() => _file.SaveConfig(DataPaths.ConfigFile, Garage.Config);
    public void ReloadPriceList() => Garage.SetPriceList(MapPriceList(_file.ReadPriceList(DataPaths.PriceListFile)));

    public Vehicle CreateVehicle(string vehicleTypeName, string regNr, DateTime checkInTime)
    {
        var price = Garage.Prices.GetPrice(vehicleTypeName);
        return vehicleTypeName switch
        {
            "Bil" => new Car(regNr, checkInTime, price),
            "MC" => new MC(regNr, checkInTime, price),
            _ => throw new ArgumentException($"Okänt fordonstyp: {vehicleTypeName}")
        };
    }

    public void SaveParkedVehicles()
    {
        var records = Garage.AllParkedVehicles().Select(x => new VehicleRecord
        {
            RegNr = x.Vehicle.RegNr,
            VehicleType = x.Vehicle.Type.ToString(),
            CheckInTime = x.Vehicle.CheckInTime,
            SpotNr = x.Spot.SpotNr,
            PricePerHour = x.Vehicle.PricePerHour,
        });
        _file.SaveVehicles(DataPaths.VehiclesFile, records);
    }
}
