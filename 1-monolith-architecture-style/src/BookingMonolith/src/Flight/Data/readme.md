dotnet ef migrations add initial --context FlightDbContext -o "Flight\Data\Migrations"
dotnet ef database update --context FlightDbContext
