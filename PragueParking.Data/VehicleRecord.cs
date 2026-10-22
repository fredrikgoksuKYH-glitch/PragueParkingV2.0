namespace PragueParking.Data;

public class VehicleRecord
{
    public string RegNr { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public int SpotNr { get; set; }
    public decimal PricePerHour { get; set; }
}

/* TODO=
 RegNr och VehicleType sätts till string.Empty; för att förhindra NRT och NRE.
 Vi behöver inte göra så för DateTime,SpotNr och PricePerHour eftersom dom
 inte kan vara null.
 Är dom toma så blir dom automatiskt en siffra t.ex:
 int (SpotNr) blir 0
 decimal (PricePerHour) blir 0.0m
 DateTime blir automatiskt 0001-01-01 00:00:00
 */