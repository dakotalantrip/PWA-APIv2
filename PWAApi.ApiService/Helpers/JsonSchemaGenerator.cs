using System.Reflection;
using System.Text.Json;
using PWAApi.ApiService.Attributes;

namespace PWAApi.ApiService.Helpers
{
    public static class JsonSchemaGenerator
    {
        public static string GenerateJsonSchema<T>()
        {
            return JsonSerializer.Serialize(GenerateJsonSchema(typeof(T)), new JsonSerializerOptions { WriteIndented = true });
        }

        public static object GenerateJsonSchema(Type type)
        {
            // Start building the JSON schema
            if (!IsCollection(type) && !IsComplexType(type))
            {
                return type.IsEnum ? new
                {
                    type = GetJsonType(type),
                    description = type.GetCustomAttribute<AIDescriptionAttribute>()?.Description ?? "",
                    @enum = Enum.GetNames(type)
                } 
                :
                new
                {
                    type = GetJsonType(type),
                    description = type.GetCustomAttribute<AIDescriptionAttribute>()?.Description ?? ""
                }; ;
            }
            else if (!IsCollection(type))
            {
                var schema = new
                {
                    type = GetJsonType(type),
                    properties = new Dictionary<string, object>(),
                    required = new List<string>(),
                    additionalProperties = false
                };

                // Iterate through the properties of the class
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetCustomAttribute<SkipAISchemaAttribute>() != null)
                        continue; // Skip properties marked with SkipAISchemaAttribute. For example, the Id property. We don't want AI setting that.

                    // Handle nested objects and collections recursively
                    var propertyType = property.PropertyType;

                    if (IsCollection(propertyType))
                    {
                        // Handle collections (e.g., arrays, lists)
                        schema.properties[property.Name] = new
                        {
                            type = "array",
                            items = GenerateJsonSchema(GetCollectionElementType(propertyType))
                        };
                    }
                    else if (IsComplexType(propertyType))
                    {
                        // Handle nested objects
                        schema.properties[property.Name] = GenerateJsonSchema(propertyType);
                    }
                    else
                    {
                        // Handle primitive types
                        schema.properties[property.Name] = propertyType.IsEnum ? new
                        {
                            type = GetJsonType(propertyType),
                            description = property.GetCustomAttribute<AIDescriptionAttribute>()?.Description ?? "",
                            @enum = Enum.GetNames(propertyType)
                        } :
                        new
                        {
                            type = GetJsonType(propertyType),
                            description = property.GetCustomAttribute<AIDescriptionAttribute>()?.Description ?? ""
                        };
                    }

                    schema.required.Add(property.Name);
                }

                return schema;
            }
            else
            {
                var elementType = GetCollectionElementType(type);
                var schema = new
                {
                    type = "object",
                    properties = new Dictionary<string, object>
                    {
                        {
                            "items", new
                            {
                                type = "array",
                                items = GenerateJsonSchema(elementType)
                            }
                        }
                    },
                    required = new List<string> { "items" },
                    additionalProperties = false
                };

                return schema;
            }
        }

        private static string GetJsonType(Type type)
        {
            // Map C# types to JSON types
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type.IsEnum) return "string";
            if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type)) return "array";
            if (type.IsClass || (type.IsValueType && !type.IsPrimitive && Nullable.GetUnderlyingType(type) == null)) return "object";

            return "string"; // Default to string for unknown types
        }

        private static bool IsNullable(PropertyInfo property)
        {
            // Check if the property is nullable
            if (!property.PropertyType.IsValueType) return true; // Reference types are nullable
            if (Nullable.GetUnderlyingType(property.PropertyType) != null) return true; // Nullable<T>
            return false; // Value types are not nullable
        }

        private static bool IsCollection(Type type)
        {
            // Check if the type is a collection (e.g., array, list)
            return type.IsArray || (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string));
        }

        private static Type GetCollectionElementType(Type collectionType)
        {
            // Get the element type of a collection (e.g., T in List<T>)
            if (collectionType.IsArray)
            {
                return collectionType.GetElementType()!;
            }

            if (collectionType.IsGenericType)
            {
                return collectionType.GetGenericArguments()[0];
            }

            return typeof(object); // Default to object if the element type cannot be determined
        }

        private static bool IsComplexType(Type type)
        {
            // Check if the type is a complex type (e.g., class, struct) but not a primitive or string
            return (type.IsClass || (type.IsValueType && !type.IsPrimitive)) && type != typeof(string) && !type.IsEnum;
        }
    }
}