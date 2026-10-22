namespace PragueParking.Core;

public class MC : Vehicle
{
    public MC(string registrationNumber, DateTime checkInTime, decimal pricePerHour)
        : base(registrationNumber, VehicleType.Bil, checkInTime, pricePerHour)
    {
    }
}