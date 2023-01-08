using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BuildingBlocks.Utils;

public static class TypeProvider
{
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
        var result = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
            .FirstOrDefault();

        return result;
    }

    public static IReadOnlyList<Assembly> GetReferencedAssemblies(Assembly? rootAssembly)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<Assembly?>();
        var listResult = new List<Assembly>();

        var root = rootAssembly ?? Assembly.GetEntryAssembly();
        queue.Enqueue(root);

        do
        {
            var asm = queue.Dequeue();

            if (asm == null)
                break;

            listResult.Add(asm);

            foreach (var reference in asm.GetReferencedAssemblies())
            {
                if (!visited.Contains(reference.FullName))
                {
                    // Load will add assembly into the application domain of the caller. loading assemblies explicitly to AppDomain, because assemblies are loaded lazily
                    // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.load
                    queue.Enqueue(Assembly.Load(reference));
                    visited.Add(reference.FullName);
                }
            }
        } while (queue.Count > 0);

        return listResult.Distinct().ToList().AsReadOnly();
    }

    public static IReadOnlyList<Assembly> GetApplicationPartAssemblies(Assembly rootAssembly)
    {
        var rootNamespace = rootAssembly.GetName().Name!.Split('.').First();
        var list = rootAssembly!.GetCustomAttributes<ApplicationPartAttribute>()
            .Where(x => x.AssemblyName.StartsWith(rootNamespace, StringComparison.InvariantCulture))
            .Select(name => Assembly.Load(name.AssemblyName))
            .Distinct();

        return list.ToList().AsReadOnly();
    }

}
