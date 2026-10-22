namespace PragueParking.Core;

public abstract class Vehicle
{
    public string RegNr { get; }
    public VehicleType Type { get; }
    public DateTime CheckInTime { get; }

    public decimal PricePerHour { get; set; }

    protected Vehicle(string regnr, VehicleType type, DateTime checkInTime, decimal pricePerHour)
    {
        if (string.IsNullOrWhiteSpace(regnr))
            throw new ArgumentNullException("Registerings nr får inte vara tomt", nameof(regnr));

        RegNr = regnr.Trim().ToUpperInvariant();
        Type = type;
        CheckInTime = checkInTime;
        PricePerHour = pricePerHour;
    }

    public decimal CalculatePrice(DateTime checkOutTime, int freeMinutesLimit)
    {
        var parkedTime = checkOutTime - CheckInTime;
        if (parkedTime.TotalMinutes < freeMinutesLimit)
            return 0m;

        var wholeHoursStarted = (int)Math.Ceiling(parkedTime.TotalMinutes / 60.0);
        return wholeHoursStarted * PricePerHour;
    }

    public override string ToString() => $"{Type} {RegNr} (incheckad {CheckInTime:yyyy-MM-dd HH:mm})";
}