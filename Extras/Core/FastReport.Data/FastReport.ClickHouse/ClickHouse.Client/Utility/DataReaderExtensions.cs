using System.Collections.Generic;
using System.Data;
using ClickHouse.Client.ADO.Readers;
using ClickHouse.Client.Types;

namespace ClickHouse.Client.Utility
{
    public static class DataReaderExtensions
    {
        public static string[] GetColumnNames(this IDataReader reader)
        {
            var count = reader.FieldCount;
            var names = new string[count];
            for (int i = 0; i < count; i++)
            {
                names[i] = reader.GetName(i);
            }

            return names;
        }

        internal static ClickHouseType[] GetClickHouseColumnTypes(this ClickHouseDataReader reader)
        {
            var count = reader.FieldCount;
            var names = new ClickHouseType[count];
            for (int i = 0; i < count; i++)
            {
                names[i] = reader.GetClickHouseType(i);
            }

            return names;
        }

        internal static IEnumerable<object[]> AsEnumerable(this IDataReader reader)
        {
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                yield return values;
            }
        }
    }
}
