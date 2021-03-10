using System;
using System.Net;
using System.Numerics;
using System.Text;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Formats
{
    internal class BinaryStreamReader : IDisposable, ISerializationTypeVisitorReader
    {
        private readonly ExtendedBinaryReader reader;

        public BinaryStreamReader(ExtendedBinaryReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Dispose() => reader.Dispose();

        public object Read(ClickHouseType type) => type.AcceptRead(this);

        public object Read(LowCardinalityType lowCardinalityType) => Read(lowCardinalityType.UnderlyingType);

        public object Read(FixedStringType fixedStringType) => Encoding.UTF8.GetString(reader.ReadBytes(fixedStringType.Length));

        public object Read(Int8Type int8Type) => reader.ReadSByte();

        public object Read(UInt32Type uInt32Type) => reader.ReadUInt32();

        public object Read(Int32Type int32Type) => reader.ReadInt32();

        public object Read(UInt16Type uInt16Type) => reader.ReadUInt16();

        public object Read(Int16Type int16Type) => reader.ReadInt16();

        public object Read(UInt8Type uInt8Type) => reader.ReadByte();

        public object Read(NothingType nothingType) => null;

        public object Read(ArrayType arrayType)
        {
            var length = reader.Read7BitEncodedInt();
            var data = arrayType.MakeArray(length);
            for (var i = 0; i < length; i++)
            {
                data.SetValue(ClearDBNull(Read(arrayType.UnderlyingType)), i);
            }
            return data;
        }

        public object Read(DateTimeType dateTimeType) => TypeConverter.DateTimeEpochStart.AddSeconds(reader.ReadUInt32());

        public object Read(DecimalType decimalType)
        {
            switch (decimalType.Size)
            {
                case 4:
                    return (decimal)reader.ReadInt32() / decimalType.Exponent;
                case 8:
                    return (decimal)reader.ReadInt64() / decimalType.Exponent;
                default:
                    var bigInt = new BigInteger(reader.ReadBytes(decimalType.Size));
                    return (decimal)bigInt / decimalType.Exponent;
            }
        }

        public object Read(NullableType nullableType) => reader.ReadByte() > 0 ? DBNull.Value : Read(nullableType.UnderlyingType);

        public object Read(TupleType tupleType)
        {
            var count = tupleType.UnderlyingTypes.Length;
            var contents = new object[count];
            for (var i = 0; i < count; i++)
            {
                var value = Read(tupleType.UnderlyingTypes[i]);
                contents[i] = ClearDBNull(value);
            }
            return tupleType.MakeTuple(contents);
        }

        public object Read(StringType stringType) => reader.ReadString();

        public object Read(UuidType uuidType)
        {
            // Byte manipulation because of ClickHouse's weird GUID/UUID implementation
            var bytes = new byte[16];
            reader.Read(bytes, 6, 2);
            reader.Read(bytes, 4, 2);
            reader.Read(bytes, 0, 4);
            reader.Read(bytes, 8, 8);
            Array.Reverse(bytes, 8, 8);
            return new Guid(bytes);
        }

        public object Read(Float32Type float32Type) => reader.ReadSingle();

        public object Read(Int64Type int64Type) => reader.ReadInt64();

        public object Read(UInt64Type uInt64Type) => reader.ReadUInt64();

        public object Read(Float64Type float64Type) => reader.ReadDouble();

        public object Read(IPv4Type pv4Type)
        {
            var ipv4bytes = reader.ReadBytes(4);
            Array.Reverse(ipv4bytes);
            return new IPAddress(ipv4bytes);
        }

        public object Read(IPv6Type pv6Type) => new IPAddress(reader.ReadBytes(16));

        public object Read(DateTime64Type dateTimeType) => TypeConverter.DateTimeEpochStart.AddTicks(MathUtils.ShiftDecimalPlaces(reader.ReadInt64(), 7 - dateTimeType.Scale));

        public object Read(DateType dateType) => TypeConverter.DateTimeEpochStart.AddDays(reader.ReadUInt16());

        public object Read(Enum8Type enumType) => enumType.Lookup(reader.ReadSByte());

        public object Read(Enum16Type enumType) => enumType.Lookup(reader.ReadInt16());

        public object Read(EnumType enumType) => enumType.Lookup(reader.ReadSByte());

        public object Read(NestedType tupleType) => throw new NotSupportedException();

        private static object ClearDBNull(object value) => value is DBNull ? null : value;
    }
}
