namespace BuildingBlocks.EFCore
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
    }

    public interface ITestDataSeeder : IDataSeeder
    {
    }
}
