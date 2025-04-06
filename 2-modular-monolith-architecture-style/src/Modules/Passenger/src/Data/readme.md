dotnet ef migrations add initial --context PassengerDbContext -o "Data\Migrations"
dotnet ef database update --context PassengerDbContext
