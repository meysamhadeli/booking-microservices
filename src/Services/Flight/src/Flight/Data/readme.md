dotnet ef migrations add Init --context FlightDbContext -o "Data\Migrations"
dotnet ef database update --context FlightDbContext
