dotnet ef migrations add initial --context FlightDbContext -o "Data\Migrations"
dotnet ef database update --context FlightDbContext
