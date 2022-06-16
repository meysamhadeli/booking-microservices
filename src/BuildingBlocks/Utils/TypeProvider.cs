using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace BuildingBlocks.Utils;

public static class TypeProvider
{
    private static readonly ConcurrentDictionary<Type, string> TypeNameMap = new();
    private static readonly ConcurrentDictionary<string, Type> TypeMap = new();
    private static bool IsRecord(this Type objectType)
    {
        return objectType.GetMethod("<Clone>$") != null ||
               ((TypeInfo)objectType)
               .DeclaredProperties.FirstOrDefault(x => x.Name == "EqualityContract")?
               .GetMethod?.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
    }

    public static Type? GetTypeFromAnyReferencingAssembly(string typeName)
    {
        var referencedAssemblies = Assembly.GetEntryAssembly()?
            .GetReferencedAssemblies()
            .Select(a => a.FullName);

        if (referencedAssemblies == null)
            return null;

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => referencedAssemblies.Contains(a.FullName))
            .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
            .FirstOrDefault();
    }

    public static Type? GetFirstMatchingTypeFromCurrentDomainAssembly(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets the type name from a generic Type class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>GetTypeName</returns>
    public static string GetTypeName<T>() => ToName(typeof(T));

    /// <summary>
    /// Gets the type name from a Type class.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>TypeName</returns>
    public static string GetTypeName(Type type) => ToName(type);

    /// <summary>
    /// Gets the type name from a instance object.
    /// </summary>
    /// <param name="o"></param>
    /// <returns>TypeName</returns>
    public static string GetTypeNameByObject(object o) => ToName(o.GetType());

    /// <summary>
    /// Gets the type class from a type name.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns>Type</returns>
    public static Type GetType(string typeName) => ToType(typeName);

    public static void AddType<T>(string name) => AddType(typeof(T), name);

    private static void AddType(Type type, string name)
    {
        ToName(type);
        ToType(name);
    }

    public static bool IsTypeRegistered<T>() => TypeNameMap.ContainsKey(typeof(T));

    private static string ToName(Type type)
    {
        Guard.Against.Null(type, nameof(type));

        return TypeNameMap.GetOrAdd(type, _ =>
        {
            var eventTypeName = type.FullName!.Replace(".", "_", StringComparison.Ordinal);

            TypeMap.GetOrAdd(eventTypeName, type);

            return eventTypeName;
        });
    }

    private static Type ToType(string typeName) => TypeMap.GetOrAdd(typeName, _ =>
    {
        Guard.Against.NullOrEmpty(typeName, nameof(typeName));

        return TypeMap.GetOrAdd(typeName, _ =>
        {
            var type = GetFirstMatchingTypeFromCurrentDomainAssembly(
                typeName.Replace("_", ".", StringComparison.Ordinal))!;

            if (type == null)
                throw new System.Exception($"Type map for '{typeName}' wasn't found!");

            return type;
        });
    });
}
