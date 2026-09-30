namespace Woo.EFCore
{
    public interface IDataSeeder
    {
        Task SeedAllAsync();
    }

    public interface ITestDataSeeder
    {
        Task SeedAllAsync();
    }
}