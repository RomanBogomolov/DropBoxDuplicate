using System;
using System.Data;

namespace DropBoxDuplicate.DataAccess.Sql.Extends
{
    /// <summary>
    /// Методы расширения для IDataReader. 
    /// Помогает разрешить конфликты, когда считываем null из таблицы в БД.
    /// </summary>
    public static class ExtendDataReader
    {
        /*
         * System.String
         */
        public static string GetStringSafe(this IDataReader reader, int colIndex)
        {
            return GetStringSafe(reader, colIndex, string.Empty);
        }

        public static string GetStringSafe(this IDataReader reader, int colIndex, string defaultValue)
        {
            return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : defaultValue;
        }

        public static string GetStringSafe(this IDataReader reader, string indexName)
        {
            return GetStringSafe(reader, reader.GetOrdinal(indexName));
        }

        public static string GetStringSafe(this IDataReader reader, string indexName, string defaultValue)
        {
            return GetStringSafe(reader, reader.GetOrdinal(indexName), defaultValue);
        }

        /*
         * System.Double
         */
        public static double GetDoubleSafe(this IDataReader reader, int colIndex)
        {
            return GetDoubleSafe(reader, colIndex, 0);
        }
        public static double GetDoubleSafe(this IDataReader reader, int colIndex, double defaultValue)
        {
            return !reader.IsDBNull(colIndex) ? reader.GetDouble(colIndex) : defaultValue;
        }
        public static double GetDoubleSafe(this IDataReader reader, string indexName)
        {
            return GetDoubleSafe(reader, reader.GetOrdinal(indexName));
        }
        public static double GetDoubleSafe(this IDataReader reader, string indexName, double defaultValue)
        {
            return GetDoubleSafe(reader, reader.GetOrdinal(indexName), defaultValue);
        }

        /*
         * System.DateTimeOffset
         */
        public static DateTimeOffset GetDateTimeOffsetSafe(this IDataReader reader, int colIndex)
        {
            return GetDateTimeOffsetSafe(reader, colIndex, default(DateTimeOffset));
        }
        public static DateTimeOffset GetDateTimeOffsetSafe(this IDataReader reader, int colIndex, DateTimeOffset defaultValue)
        {
            return !reader.IsDBNull(colIndex) ? reader.GetDateTime(colIndex) : defaultValue;
        }
        public static DateTimeOffset GetDateTimeOffsetSafe(this IDataReader reader, string indexName)
        {
            return GetDateTimeOffsetSafe(reader, reader.GetOrdinal(indexName));
        }
        public static DateTimeOffset GetDateTimeOffsetSafe(this IDataReader reader, string indexName, DateTimeOffset defaultValue)
        {
            return GetDateTimeOffsetSafe(reader, reader.GetOrdinal(indexName), defaultValue);
        }
    }
}