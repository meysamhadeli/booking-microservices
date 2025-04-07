dotnet ef migrations add initial --context IdentityContext -o "Identity\Data\Migrations"
dotnet ef database update --context IdentityContext
