dotnet ef migrations add initial --context PersistMessageDbContext -o "Data\Migrations"
dotnet ef database update --context PersistMessageDbContext
