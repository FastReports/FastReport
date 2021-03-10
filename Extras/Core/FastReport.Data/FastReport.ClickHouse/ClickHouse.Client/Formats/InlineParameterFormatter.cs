using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Formats
{
    public static class InlineParameterFormatter
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
            switch (type.TypeCode)
            {
                case ClickHouseTypeCode.UInt8:
                case ClickHouseTypeCode.UInt16:
                case ClickHouseTypeCode.UInt32:
                case ClickHouseTypeCode.UInt64:
                case ClickHouseTypeCode.Int8:
                case ClickHouseTypeCode.Int16:
                case ClickHouseTypeCode.Int32:
                case ClickHouseTypeCode.Int64:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);

                case ClickHouseTypeCode.Float32:
                case ClickHouseTypeCode.Float64:
                    return FormatFloat(value);

                case ClickHouseTypeCode.Decimal:
                    return FormatDecimal(type, value);

                case ClickHouseTypeCode.String:
                case ClickHouseTypeCode.FixedString:
                case ClickHouseTypeCode.LowCardinality:
                case ClickHouseTypeCode.Enum8:
                case ClickHouseTypeCode.Enum16:
                    return value.ToString().Escape();
                case ClickHouseTypeCode.UUID:
                    return $"toUUID({value.ToString().Escape()})";

                case ClickHouseTypeCode.Nothing:
                    return "null";

                case ClickHouseTypeCode.Date when value is DateTime:
                    return $"toDate('{value:yyyy-MM-dd}')";

                case ClickHouseTypeCode.DateTime when type is AbstractDateTimeType dateTimeType && value is DateTime dateTime:
                    if (dateTimeType.TimeZone != null)
                    {
                        dateTime = dateTime.ToUniversalTime();
                    }
                    return $"toDateTime('{dateTime:yyyy-MM-dd HH:mm:ss}')";

                case ClickHouseTypeCode.DateTime64 when type is DateTime64Type dateTimeType && value is DateTime dateTime:
                    if (dateTimeType.TimeZone != null)
                        dateTime = dateTime.ToUniversalTime();
                    return $"toDateTime64('{dateTime:yyyy-MM-dd HH:mm:ss.fffffff}', 7)";

                case ClickHouseTypeCode.IPv4: return $"toIPv4({FormatIPAddress(value)})";
                case ClickHouseTypeCode.IPv6: return $"toIPv6({FormatIPAddress(value)})";

                case ClickHouseTypeCode.Nullable:
                    var nullableType = (NullableType)type;
                    return value is null || value is DBNull ? "null" : $"{Format(nullableType.UnderlyingType, value)}";

                case ClickHouseTypeCode.Array:
                    var arrayType = (ArrayType)type;
                    var array = ((IEnumerable)value).Cast<object>().Select(obj => Format(arrayType.UnderlyingType, obj));
                    return $"[{string.Join(",", array)}]";

                case ClickHouseTypeCode.Tuple:
                    var tupleType = (TupleType)type;
                    var tuple = (ITuple)value;
                    return $"({string.Join(",", tupleType.UnderlyingTypes.Select((x, i) => Format(x, tuple[i])))})";

                default:
                    throw new NotSupportedException($"Cannot convert value {value} to type {type.TypeCode}");
            }
        }

        private static object FormatIPAddress(object value) => value switch
        {
            IPAddress ipAddress => ipAddress.ToString().Escape(),
            string str => str,
            _ => throw new NotSupportedException($"Cannot convert value {value} to IP address")
        };

        private static string FormatFloat(object value) => value switch
        {
            float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
            double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
            _ => Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture)
        };

        private static string FormatDecimal(ClickHouseType type, object value)
        {
            if (!(value is decimal decimalValue))
                decimalValue = Convert.ToDecimal(value);
            return type switch
            {
                Decimal128Type decimal128Type => $"toDecimal128({decimalValue.ToString(CultureInfo.InvariantCulture)},{decimal128Type.Scale})",
                Decimal64Type decimal64Type => $"toDecimal64({decimalValue.ToString(CultureInfo.InvariantCulture)},{decimal64Type.Scale})",
                Decimal32Type decimal32Type => $"toDecimal32({decimalValue.ToString(CultureInfo.InvariantCulture)},{decimal32Type.Scale})",
                DecimalType _ => decimalValue.ToString(CultureInfo.InvariantCulture),
                _ => throw new NotSupportedException($"Cannot convert value {value} to decimal type")
            };
        }
    }
}
