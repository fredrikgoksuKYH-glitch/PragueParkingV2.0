# Dokumentation

Har gjort om projektet för att jag inte blir nöjd, antigen blir det för
komplicerat eller för simpelt.

# Planering
Separera allt i Libraryies istället för bara maps, så man
kan starta Prague Parking 2 och Prague Parking 2.1 ( om jag bestämmer mig för göra 2.1).

# Planering

	PragueParkingApp
	|--Data
	|	|--config.json
	|	|--parkedvehicles.json
	|	|--pricelist.txt
	|
	|-DataPaths.cs
	|-GarageService.cs
	|-GarageView.cs
	|-Program.cs

	PragueParkingData
	|-MyFiles.cs
	|-PriceListData.cs
	|-VehicleRecord.cs

	PragueParkingCore
	|-VehicleType.cs	-Innehåller Enum som definerar giltiga fordonstyper
	|-Vehicle.cs		-Abstrakt Klass för alla fordon("mallen")
	||-Car.cs		-Subklass
	||-MC.cs		-Subklass
	|-GarageConfig.cs
	|-ParkingGarage.cs
	|-ParkingSpot.cs	
	|-PriceList.cs

# Checklista 
	PPCore:
				||	|Dag|
	VehcileType.cs		|✓|	|1|	||
	Vehicle.cs		|✓|	|1|	||
	Car.cs & MC.cs		|✓|	|1|	||
	GarageConfig.cs		|✓|	|1|	||
	PriceList.cs		|✓|	|1|	||
	ParkingSpot.cs		|✓|	|2|	||
	ParkingGarage.cs	|✓|	|2|	||

	PPData:
	VehicleRecord.cs	|✓|	|2|	||
	PriceListData.cs	|✓|	||	||
	MyFiles.cs		|✓|	||	||

	PPApp:
	Datapath.cs		|✓|	||	||
	config.json		|✓|	||	||
	pricelist.txt		|✓|	||	||
	parkedvehicles.json	|✓|	||	||

	GarageService.cs	|✓|	||	||
	GarageView.cs		|✓|	||	||
	Program.cs		|✓|	||	||


PPCore:
1. VehicleType görs först och en enum för att Claes hinta på att det
gick och göra så under lektion ;) så vi sätter in ett enum.
Vi gör denna först för basklasser och logiken vi ska byta behöver veta
vilka fordonstyper som finns

2.Vehicle.cs sätts till abstract och car och mc kommer att ärva av Vehicle.
Vi hade förmodligen kunna gjort allt till olika klasser men blivit jobbigt
att ha en lista för mc,car osv.

3.GarageConfig & Pricelist.cs här börjar dom härliga => ;)
har försökt att hålla dom så korta som möjligt och ibland även skrivit ut dom helt.

4. ParkingSpot.cs
4. ParkingGarage.cs
4. VehicleRecord.cs
4. PriceListData.cs
4. MyFiles.cs
4. DataPaths.cs
4. config.json,pricelist.txt & parkedvehicles.json
4. GarageService.cs
4. GarageView.cs
4. Program.cs

