using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.Types;

namespace ClickHouse.Client.Formats
{
    public static class HttpParameterFormatter
    {
        public static string Format(ClickHouseDbParameter parameter)
        {
            if (parameter.Value is null)
                return string.Empty;
            var type = string.IsNullOrWhiteSpace(parameter.ClickHouseType)
                ? TypeConverter.ToClickHouseType(parameter.Value.GetType())
                : TypeConverter.ParseClickHouseType(parameter.ClickHouseType);
            return Format(type, parameter.Value);
        }

        internal static string Format(ClickHouseType type, object value)
        {
            return type.TypeCode switch
            {
                var simpleType when
                    simpleType == ClickHouseTypeCode.UInt8 ||
                    simpleType == ClickHouseTypeCode.UInt16 ||
                    simpleType == ClickHouseTypeCode.UInt32 ||
                    simpleType == ClickHouseTypeCode.UInt64 ||
                    simpleType == ClickHouseTypeCode.Int8 ||
                    simpleType == ClickHouseTypeCode.Int16 ||
                    simpleType == ClickHouseTypeCode.Int32 ||
                    simpleType == ClickHouseTypeCode.Int64 => Convert.ToString(value, CultureInfo.InvariantCulture),

                var floatType when
                    floatType == ClickHouseTypeCode.Float32 ||
                    floatType == ClickHouseTypeCode.Float64 => FormatFloat(value),

                ClickHouseTypeCode.Decimal when value is decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),

                var stringType when
                    stringType == ClickHouseTypeCode.String ||
                    stringType == ClickHouseTypeCode.FixedString ||
                    stringType == ClickHouseTypeCode.LowCardinality ||
                    stringType == ClickHouseTypeCode.Enum8 ||
                    stringType == ClickHouseTypeCode.Enum16 ||
                    stringType == ClickHouseTypeCode.UUID ||
                    stringType == ClickHouseTypeCode.IPv4 ||
                    stringType == ClickHouseTypeCode.IPv6 => value.ToString(),

                ClickHouseTypeCode.Nothing => $"null",

                ClickHouseTypeCode.Date when value is DateTime date => $"{date:yyyy-MM-dd}",
                ClickHouseTypeCode.DateTime when type is DateTimeType dateTimeType && value is DateTime dateTime =>
                    dateTimeType.TimeZone == null
                        ? $"{dateTime:yyyy-MM-dd HH:mm:ss}"
                        : $"{dateTime.ToUniversalTime():yyyy-MM-dd HH:mm:ss}",
                ClickHouseTypeCode.DateTime64 when type is DateTime64Type dateTimeType && value is DateTime dateTime =>
                    dateTimeType.TimeZone == null
                        ? $"{dateTime:yyyy-MM-dd HH:mm:ss.fffffff}"
                        : $"{dateTime.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}",

                ClickHouseTypeCode.Nullable when type is NullableType nullableType => value is null || value is DBNull ? "null" : $"{Format(nullableType.UnderlyingType, value)}",

                ClickHouseTypeCode.Array when type is ArrayType arrayType && value is IEnumerable enumerable =>
                    $"[{string.Join(",", enumerable.Cast<object>().Select(obj => InlineParameterFormatter.Format(arrayType.UnderlyingType, obj)))}]",

                ClickHouseTypeCode.Tuple when type is TupleType tupleType && value is ITuple tuple =>
                $"({string.Join(",", tupleType.UnderlyingTypes.Select((x, i) => InlineParameterFormatter.Format(x, tuple[i])))})",

                _ => throw new NotSupportedException($"Cannot convert value {value} to type {type.TypeCode}")
            };
        }

        private static string FormatFloat(object value)
        {
            return value switch
            {
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
                _ => throw new NotSupportedException($"Cannot convert value {value} to float type")
            };
        }
    }
}
