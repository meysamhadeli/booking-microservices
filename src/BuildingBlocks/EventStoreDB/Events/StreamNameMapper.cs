using System.Collections.Concurrent;

namespace BuildingBlocks.EventStoreDB.Events;

public class StreamNameMapper
{
    private static readonly StreamNameMapper Instance = new();

    private readonly ConcurrentDictionary<Type, string> TypeNameMap = new();

    public static void AddCustomMap<TStream>(string mappedStreamName) =>
        AddCustomMap(typeof(TStream), mappedStreamName);

    public static void AddCustomMap(Type streamType, string mappedStreamName)
    {
        Instance.TypeNameMap.AddOrUpdate(streamType, mappedStreamName, (_, _) => mappedStreamName);
    }
    public static string ToStreamId<TStream>(object aggregateId, object? tenantId = null) =>
        ToStreamId(typeof(TStream), aggregateId);

    public static string ToStreamId(Type streamType, object aggregateId, object? tenantId = null)
    {
        var tenantPrefix = tenantId != null ? $"{tenantId}_"  : "";

        return $"{tenantPrefix}{streamType.Name}-{aggregateId}";
    }

}
