namespace Woo.EFCore;

public interface ISeedManager
{
    Task ExecuteSeedAsync();
    Task ExecuteTestSeedAsync();
}