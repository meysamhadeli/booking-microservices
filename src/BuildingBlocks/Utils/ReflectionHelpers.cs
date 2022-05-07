using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Utils
{
    public static class ReflectionHelpers
    {
        private static readonly ConcurrentDictionary<Type, string> TypeCacheKeys = new();
        private static readonly ConcurrentDictionary<Type, string> PrettyPrintCache = new();

        public static string GetCacheKey(this Type type)
        {
            return TypeCacheKeys.GetOrAdd(type, t => $"{t.PrettyPrint()}");
        }
        public static string PrettyPrint(this Type type)
        {
            return PrettyPrintCache.GetOrAdd(
                type,
                t =>
                {
                    try
                    {
                        return PrettyPrintRecursive(t, 0);
                    }
                    catch (System.Exception)
                    {
                        return t.Name;
                    }
                });
        }
        
        public static bool IsActionDelegate(this Type sourceType)
        {
            if (sourceType.IsSubclassOf(typeof(MulticastDelegate)) &&
                sourceType.GetMethod("Invoke").ReturnType == typeof(void))
                return true;
            return false;
        }
        private static string PrettyPrintRecursive(Type type, int depth)
        {
            if (depth > 3)
            {
                return type.Name;
            }

            var nameParts = type.Name.Split('`');
            if (nameParts.Length == 1)
            {
                return nameParts[0];
            }

            var genericArguments = type.GetTypeInfo().GetGenericArguments();
            return !type.IsConstructedGenericType
                ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
                : $"{nameParts[0]}<{string.Join(",", genericArguments.Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
        }
    }
}