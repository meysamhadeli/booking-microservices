dotnet ef migrations add initial --context PassengerDbContext -o "Passenger\Data\Migrations"
dotnet ef database update --context PassengerDbContext
