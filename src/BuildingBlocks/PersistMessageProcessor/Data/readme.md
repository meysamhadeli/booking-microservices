dotnet ef migrations add initial --context PersistMessageDbContext -o "PersistMessageProcessor\Data\Migrations"
dotnet ef database update --context PersistMessageDbContext
