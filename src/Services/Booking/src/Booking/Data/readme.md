dotnet ef migrations add initial --context BookingDbContext -o "Data\Migrations"
dotnet ef database update --context BookingDbContext
