namespace PragueParking.Core;

public class Car : Vehicle
{
public Car(string regNr,DateTime checkInTime, decimal pricePerHour)
    : base(regNr, VehicleType.Bil, checkInTime, pricePerHour)
    {}
}