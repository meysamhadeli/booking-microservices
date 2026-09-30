namespace Woo.Core.Model;

// For handling optimistic concurrency
public interface IVersion
{
    long Version { get; set; }
}