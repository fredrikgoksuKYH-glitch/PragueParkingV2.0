using PragueParking.Core;
using PragueParkingApp;
using Spectre.Console;

var service = new GarageService();

AnsiConsole.Write(new FigletText("Prague Parking").Color(Color.SteelBlue1));
AnsiConsole.MarkupLine("[grey]Version 2.0 – OOP-version[/]");
AnsiConsole.MarkupLine($"[grey]{service.Garage.TotalSpots} P-platser inlästa från konfigurationsfilen.[/]");
AnsiConsole.WriteLine();

bool running = true;
while (running)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Vad vill du göra?")
            .PageSize(10)
            .AddChoices(new[]
            {
                "1. Checka in fordon",
                "2. Checka ut fordon",
                "3. Visa karta över P-huset",
                "4. Lista alla parkerade fordon",
                "5. Sök fordon",
                "6. Flytta fordon",
                "7. Läs om prislistan",
                "8. Avsluta"
            }));

    AnsiConsole.Clear();
    switch (choice[..1])
    {
        case "1": CheckIn(service); break;
        case "2": CheckOut(service); break;
        case "3": GarageView.Show(service.Garage); break;
        case "4": ListVehicles(service.Garage); break;
        case "5": SearchVehicle(service.Garage); break;
        case "6": MoveVehicle(service); break;
        case "7": ReloadPriceList(service); break;
        case "8": running = false; break;
    }

    if (running)
    {
        AnsiConsole.MarkupLine("\n[grey](Tryck på en tangent för att fortsätta...)[/]");
        Console.ReadKey(true);
        AnsiConsole.Clear();
    }
}

AnsiConsole.MarkupLine("[grey]Hej då![/]");
return;

static void CheckIn(GarageService service)
{
    AnsiConsole.Write(new Rule("[bold]Checka in fordon[/]").LeftJustified());

    var type = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Fordonstyp?")
            .AddChoices(service.Garage.Config.VehiclesPerSpot.Keys));

    var reg = AnsiConsole.Ask<string>("Registreringsnummer?").Trim().ToUpperInvariant();

    if (service.Garage.IsRegNrParked(reg))
    {
        AnsiConsole.MarkupLine($"[red]Fordonet {reg} är redan incheckat![/]");
        return;
    }

    var vehicle = service.CreateVehicle(type, reg, DateTime.Now);
    var spot = service.Garage.CheckIn(vehicle);

    if (spot is null)
    {
        AnsiConsole.MarkupLine($"[red]Tyvärr, garaget är fullt för fordonstypen {type}.[/]");
        return;
    }

    service.SaveParkedVehicles();
    AnsiConsole.MarkupLine($"[green]{vehicle} incheckad på plats {spot.SpotNr}.[/]");
}

static void CheckOut(GarageService service)
{
    AnsiConsole.Write(new Rule("[bold]Checka ut fordon[/]").LeftJustified());
    var reg = AnsiConsole.Ask<string>("Registreringsnummer?").Trim().ToUpperInvariant();

    var result = service.Garage.CheckOut(reg, DateTime.Now);
    if (result is null)
    {
        AnsiConsole.MarkupLine($"[red]Hittade inget fordon med registreringsnummer {reg}.[/]");
        return;
    }

    service.SaveParkedVehicles();

    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("Kvitto");
    table.AddColumn("");
    table.AddRow("Registreringsnummer", result.Vehicle.RegNr);
    table.AddRow("Plats", result.SpotNr.ToString());
    table.AddRow("Incheckad", result.Vehicle.CheckInTime.ToString("yyyy-MM-dd HH:mm"));
    table.AddRow("Utcheckad", result.CheckOutTime.ToString("yyyy-MM-dd HH:mm"));
    table.AddRow("Parkerad tid", $"{(int)result.ParkedDuration.TotalHours}h {result.ParkedDuration.Minutes}min");
    table.AddRow("[bold]Att betala[/]", $"[bold]{result.Price} CZK[/]");
    AnsiConsole.Write(table);
}

static void ListVehicles(ParkingGarage garage)
{
    AnsiConsole.Write(new Rule("[bold]Parkerade fordon[/]").LeftJustified());
    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("Plats");
    table.AddColumn("Typ");
    table.AddColumn("Regnr");
    table.AddColumn("Incheckad");
    table.AddColumn("Parkerad tid");

    foreach (var (spot, vehicle) in garage.AllParkedVehicles())
    {
        var duration = DateTime.Now - vehicle.CheckInTime;
        table.AddRow(
            spot.SpotNr.ToString(),
            vehicle.Type.ToString(),
            vehicle.RegNr,
            vehicle.CheckInTime.ToString("yyyy-MM-dd HH:mm"),
            $"{(int)duration.TotalHours}h {duration.Minutes}min");
    }

    if (table.Rows.Count == 0)
        AnsiConsole.MarkupLine("[grey]Garaget är tomt.[/]");
    else
        AnsiConsole.Write(table);
}

static void SearchVehicle(ParkingGarage garage)
{
    AnsiConsole.Write(new Rule("[bold]Sök fordon[/]").LeftJustified());
    var reg = AnsiConsole.Ask<string>("Registreringsnummer?").Trim().ToUpperInvariant();

    var found = garage.FindVehicle(reg);
    if (found is null)
    {
        AnsiConsole.MarkupLine($"[red]Hittade inget fordon med registreringsnummer {reg}.[/]");
        return;
    }

    var (spot, vehicle) = found.Value;
    var duration = DateTime.Now - vehicle.CheckInTime;
    AnsiConsole.MarkupLine($"[green]{vehicle.RegNr}[/] ({vehicle.Type}) står på plats [bold]{spot.SpotNr}[/].");
    AnsiConsole.MarkupLine($"Incheckad: {vehicle.CheckInTime:yyyy-MM-dd HH:mm} ({(int)duration.TotalHours}h {duration.Minutes}min sedan)");
}

static void MoveVehicle(GarageService service)
{
    AnsiConsole.Write(new Rule("[bold]Flytta fordon[/]").LeftJustified());
    var reg = AnsiConsole.Ask<string>("Registreringsnummer på fordonet som ska flyttas?").Trim().ToUpperInvariant();
    var newSpot = AnsiConsole.Ask<int>("Till vilken P-plats (nummer)?");

    var error = service.Garage.MoveVehicle(reg, newSpot);
    if (error is not null)
    {
        AnsiConsole.MarkupLine($"[red]{error}[/]");
        return;
    }

    service.SaveParkedVehicles();
    AnsiConsole.MarkupLine($"[green]{reg} flyttades till plats {newSpot}.[/]");
}

static void ReloadPriceList(GarageService service)
{
    AnsiConsole.Write(new Rule("[bold]Läs om prislistan[/]").LeftJustified());
    service.ReloadPriceList();
    AnsiConsole.MarkupLine("[green]Prislistan har lästs om.[/] (Gäller nya incheckningar; redan parkerade fordon behåller sitt pris.)");

    var table = new Table().Border(TableBorder.Rounded);
    table.AddColumn("Fordonstyp");
    table.AddColumn("Pris/timme (CZK)");
    foreach (var kv in service.Garage.Prices.PricePerHour)
        table.AddRow(kv.Key, kv.Value.ToString());
    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine($"Fri parkering under: {service.Garage.Prices.FreeMinutesLimit} minuter.");
}
