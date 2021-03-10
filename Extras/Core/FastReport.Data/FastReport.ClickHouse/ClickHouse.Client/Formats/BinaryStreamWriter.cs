using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Formats
{
    internal class BinaryStreamWriter : IDisposable, ISerializationTypeVisitorWriter
    {
        private readonly ExtendedBinaryWriter writer;

        public BinaryStreamWriter(ExtendedBinaryWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Dispose() => writer.Dispose();

        internal void Write(ClickHouseType clickHouseType, object value) => clickHouseType.AcceptWrite(this, value);

        private void WriteLargeDecimal(DecimalType dti, decimal value)
        {
            var bigInt = new BigInteger(value);
            byte[] bigIntBytes = bigInt.ToByteArray();
            byte[] decimalBytes = new byte[dti.Size];
            bigIntBytes.CopyTo(decimalBytes, 0);

            // If a negative BigInteger is not long enough to fill the whole buffer, the remainder needs to be filled with 0xFF
            if (bigInt < 0)
            {
                for (int i = bigIntBytes.Length; i < dti.Size; i++)
                    decimalBytes[i] = 0xFF;
            }
            writer.Write(decimalBytes);
        }

        public void Write(LowCardinalityType lowCardinalityType, object value) => Write(lowCardinalityType.UnderlyingType, value);

        public void Write(FixedStringType fixedStringType, object value)
        {
            var @string = Convert.ToString(value, CultureInfo.InvariantCulture);
            var stringBytes = new byte[fixedStringType.Length];
            Encoding.UTF8.GetBytes(@string, 0, @string.Length, stringBytes, 0);
            writer.Write(stringBytes);
        }

        public void Write(Int8Type int8Type, object value) => writer.Write(Convert.ToSByte(value));

        public void Write(UInt32Type uInt32Type, object value) => writer.Write(Convert.ToUInt32(value));

        public void Write(Enum8Type enumType, object value)
        {
            var enumIndex = value is string enumStr ? (sbyte)enumType.Lookup(enumStr) : Convert.ToSByte(value);
            writer.Write(enumIndex);
        }

        public void Write(Int32Type int32Type, object value) => writer.Write(Convert.ToInt32(value));

        public void Write(Float32Type float32Type, object value) => writer.Write(Convert.ToSingle(value));

        public void Write(IPv4Type pv4Type, object value)
        {
            var address4 = ExtractIPAddress(value);
            if (address4.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentException($"Expected IPv4, got {address4.AddressFamily}");
            }

            var ipv4bytes = address4.GetAddressBytes();
            Array.Reverse(ipv4bytes);
            writer.Write(ipv4bytes, 0, ipv4bytes.Length);
        }

        public void Write(Float64Type float64Type, object value) => writer.Write(Convert.ToDouble(value));

        public void Write(StringType stringType, object value) => writer.Write(Convert.ToString(value, CultureInfo.InvariantCulture));

        public void Write(UuidType uuidType, object value)
        {
            var guid = ExtractGuid(value);
            var bytes = guid.ToByteArray();
            Array.Reverse(bytes, 8, 8);
            writer.Write(bytes, 6, 2);
            writer.Write(bytes, 4, 2);
            writer.Write(bytes, 0, 4);
            writer.Write(bytes, 8, 8);
        }

        public void Write(Enum16Type enumType, object value)
        {
            var enumIndex = value is string enumStr ? (short)enumType.Lookup(enumStr) : Convert.ToInt16(value);
            writer.Write(enumIndex);
        }

        public void Write(UInt16Type uInt16Type, object value) => writer.Write(Convert.ToUInt16(value));

        public void Write(NothingType nothingType, object value) { }

        public void Write(Int16Type int16Type, object value) => writer.Write(Convert.ToInt16(value));

        public void Write(UInt8Type uInt8Type, object value) => writer.Write(Convert.ToByte(value));

        public void Write(Int64Type int64Type, object value) => writer.Write(Convert.ToInt64(value));

        public void Write(UInt64Type uInt64Type, object value) => writer.Write(Convert.ToUInt64(value));

        public void Write(ArrayType arrayType, object value)
        {
            var collection = (IList)value;
            writer.Write7BitEncodedInt(collection.Count);
            for (var i = 0; i < collection.Count; i++)
            {
                Write(arrayType.UnderlyingType, collection[i]);
            }
        }

        public void Write(DateTime64Type dateTime64Type, object value)
        {
            var dateTimeOffset = dateTime64Type.ToDateTimeOffset((DateTime)value);
            var ticks = (dateTimeOffset.UtcDateTime - TypeConverter.DateTimeEpochStart).Ticks;
            // 7 is a 'magic constant' - Log10 of TimeSpan.TicksInSecond
            writer.Write(MathUtils.ShiftDecimalPlaces(ticks, dateTime64Type.Scale - 7));
        }

        public void Write(NullableType nullableType, object value)
        {
            if (value == null || value is DBNull)
            {
                writer.Write((byte)1);
            }
            else
            {
                writer.Write((byte)0);
                Write(nullableType.UnderlyingType, value);
            }
        }

        public void Write(IPv6Type pv6Type, object value)
        {
            var address6 = ExtractIPAddress(value);
            if (address6.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException($"Expected IPv6, got {address6.AddressFamily}");
            }

            var ipv6bytes = address6.GetAddressBytes();
            writer.Write(ipv6bytes, 0, ipv6bytes.Length);
        }

        public void Write(TupleType tupleType, object value)
        {
            var tuple = (ITuple)value;
            for (var i = 0; i < tuple.Length; i++)
            {
                Write(tupleType.UnderlyingTypes[i], tuple[i]);
            }
        }

        public void Write(DateTimeType dateTimeType, object value)
        {
            var dateTimeOffset = dateTimeType.ToDateTimeOffset((DateTime)value);
            var seconds = (uint)(dateTimeOffset.UtcDateTime - TypeConverter.DateTimeEpochStart).TotalSeconds;
            writer.Write(seconds);
        }

        public void Write(DecimalType decimalType, object value)
        {
            decimal multipliedValue = Convert.ToDecimal(value) * decimalType.Exponent;
            switch (decimalType.Size)
            {
                case 4:
                    writer.Write((int)multipliedValue);
                    break;
                case 8:
                    writer.Write((long)multipliedValue);
                    break;
                default:
                    WriteLargeDecimal(decimalType, multipliedValue);
                    break;
            }
        }

        public void Write(DateType dateTimeType, object value)
        {
            var days = (ushort)(((DateTime)value).Date - TypeConverter.DateTimeEpochStart).TotalDays;
            writer.Write(days);
        }

        public void Write(EnumType enumType, object value)
        {
            var enumIndex = value is string enumStr ? (sbyte)enumType.Lookup(enumStr) : Convert.ToSByte(value);
            writer.Write(enumIndex);
        }

        public void Write(NestedType nestedType, object value) => throw new NotSupportedException("Writing Nested values directly is not supported, see documentation");

        private static IPAddress ExtractIPAddress(object data) => data is IPAddress a ? a : IPAddress.Parse((string)data);

        private static Guid ExtractGuid(object data) => data is Guid g ? g : new Guid((string)data);
    }
}
