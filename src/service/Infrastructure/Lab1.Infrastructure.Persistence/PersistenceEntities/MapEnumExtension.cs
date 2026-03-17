using System.Data;
using System.Data.Common;

namespace Lab1.Infrastructure.Persistence.PersistenceEntities;

public static class MapEnumExtension
{
    public static T MapEnum<T>(this DbDataReader dataReader, string name) where T : struct, Enum
    {
        string stringValue = dataReader.GetString(name);
        if (!Enum.TryParse<T>(stringValue, true, out T result))
        {
            throw new InvalidOperationException("Cannot map value: " + stringValue + " to enum " + typeof(T).Name);
        }

        return result;
    }
}