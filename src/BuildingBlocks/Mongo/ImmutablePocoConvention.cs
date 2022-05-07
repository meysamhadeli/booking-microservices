using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace BuildingBlocks.Mongo
{
    /// <summary>
    /// A convention that map all read only properties for which a matching constructor is found.
    /// Also matching constructors are mapped.
    /// </summary>
    public class ImmutablePocoConvention : ConventionBase, IClassMapConvention
    {
        private readonly BindingFlags _bindingFlags;

        public ImmutablePocoConvention()
            : this(BindingFlags.Instance | BindingFlags.Public)
        {
        }

        public ImmutablePocoConvention(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
        }

        public void Apply(BsonClassMap classMap)
        {
            var readOnlyProperties = classMap.ClassType.GetTypeInfo()
                .GetProperties(_bindingFlags)
                .Where(p => IsReadOnlyProperty(classMap, p))
                .ToList();

            foreach (var constructor in classMap.ClassType.GetConstructors())
            {
                // If we found a matching constructor then we map it and all the readonly properties
                var matchProperties = GetMatchingProperties(constructor, readOnlyProperties);
                if (matchProperties.Any())
                {
                    // Map constructor
                    classMap.MapConstructor(constructor);

                    // Map properties
                    foreach (var p in matchProperties)
                        classMap.MapMember(p);
                }
            }
        }

        private static List<PropertyInfo> GetMatchingProperties(
            ConstructorInfo constructor,
            List<PropertyInfo> properties)
        {
            var matchProperties = new List<PropertyInfo>();

            var ctorParameters = constructor.GetParameters();
            foreach (var ctorParameter in ctorParameters)
            {
                var matchProperty = properties.FirstOrDefault(p => ParameterMatchProperty(ctorParameter, p));
                if (matchProperty == null)
                    return new List<PropertyInfo>();

                matchProperties.Add(matchProperty);
            }

            return matchProperties;
        }


        private static bool ParameterMatchProperty(ParameterInfo parameter, PropertyInfo property)
        {
            return string.Equals(property.Name, parameter.Name, System.StringComparison.InvariantCultureIgnoreCase)
                   && parameter.ParameterType == property.PropertyType;
        }

        private static bool IsReadOnlyProperty(BsonClassMap classMap, PropertyInfo propertyInfo)
        {
            // we can't read
            if (!propertyInfo.CanRead)
                return false;

            // we can write (already handled by the default convention...)
            if (propertyInfo.CanWrite)
                return false;

            // skip indexers
            if (propertyInfo.GetIndexParameters().Length != 0)
                return false;

            // skip overridden properties (they are already included by the base class)
            var getMethodInfo = propertyInfo.GetMethod;
            if (getMethodInfo.IsVirtual && getMethodInfo.GetBaseDefinition().DeclaringType != classMap.ClassType)
                return false;

            return true;
        }
    }
}
