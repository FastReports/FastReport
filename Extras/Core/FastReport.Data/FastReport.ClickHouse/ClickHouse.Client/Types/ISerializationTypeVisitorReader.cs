namespace ClickHouse.Client.Types
{
    internal interface ISerializationTypeVisitorReader
    {
        object Read(LowCardinalityType lowCardinalityType);

        object Read(FixedStringType fixedStringType);

        object Read(Int8Type int8Type);

        object Read(UInt32Type uInt32Type);

        object Read(Int32Type int32Type);

        object Read(DateType dateType);

        object Read(UInt16Type uInt16Type);

        object Read(Int16Type int16Type);

        object Read(NothingType nothingType);

        object Read(UInt8Type uInt8Type);

        object Read(ArrayType arrayType);

        object Read(DateTimeType dateTimeType);

        object Read(DateTime64Type dateTimeType);

        object Read(DecimalType decimalType);

        object Read(NullableType nullableType);

        object Read(Enum8Type enumType);

        object Read(Enum16Type enumType);

        object Read(TupleType tupleType);

        object Read(NestedType nestedType);

        object Read(IPv4Type pv4Type);

        object Read(Float32Type float32Type);

        object Read(Int64Type int64Type);

        object Read(UuidType uuidType);

        object Read(StringType stringType);

        object Read(UInt64Type uInt64Type);

        object Read(Float64Type float64Type);

        object Read(IPv6Type pv6Type);

        object Read(EnumType enumType);
    }
}
