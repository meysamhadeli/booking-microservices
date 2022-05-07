namespace BuildingBlocks.EFCore
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
    }
}