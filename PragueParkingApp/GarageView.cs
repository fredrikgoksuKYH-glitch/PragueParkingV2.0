using PragueParking.Core;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParkingApp;

public static class GarageView
{
    private const int ColumnsPerRow = 10;
    public static void Show(ParkingGarage garage)
    {
        AnsiConsole.Write(new Rule("[bold]Karta över P-huset[/]").LeftJustified());

        var grid = new Grid();
        for (int i = 0; i < ColumnsPerRow; i++)
            grid.AddColumn();

        var row = new List<IRenderable>();
        foreach (var spot in garage.Spots.OrderBy(s => s.SpotNr))
        {
            row.Add(new Markup(RenderSpot(spot)));
            if (row.Count == ColumnsPerRow)
            {
                grid.AddRow(row.ToArray());
                row = new List<IRenderable>();
            }
        }
        if (row.Count > 0)
        {
            while (row.Count < ColumnsPerRow) row.Add(new Markup(" "));
            grid.AddRow(row.ToArray());
        }

        AnsiConsole.Write(grid);
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[green]■[/] = Ledig    [red]■[/] = Bil    [yellow]■[/] = MC");
        AnsiConsole.MarkupLine($"Lediga platser: [green]{garage.FreeSpots}[/] / {garage.TotalSpots}    " +
                                $"Upptagna: [red]{garage.OccupiedSpots}[/] / {garage.TotalSpots}");
    }

    private static string RenderSpot(ParkingSpot spot)
    {
        if (spot.IsEmpty)
            return $"[green]■[/] {spot.SpotNr,3}";

        var color = spot.OccupyingType == VehicleType.Bil ? "red" : "yellow";
        return $"[{color}]■[/] {spot.SpotNr,3}";
    }
}
