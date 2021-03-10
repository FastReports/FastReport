namespace ClickHouse.Client.Types
{
    internal interface ISerializationTypeVisitorWriter
    {
        void Write(LowCardinalityType lowCardinalityType, object value);

        void Write(Int8Type int8Type, object value);

        void Write(UInt32Type uInt32Type, object value);

        void Write(Enum8Type enum8Type, object value);

        void Write(Int32Type int32Type, object value);

        void Write(Float32Type float32Type, object value);

        void Write(IPv4Type pv4Type, object value);

        void Write(Float64Type float64Type, object value);

        void Write(StringType stringType, object value);

        void Write(UuidType uuidType, object value);

        void Write(Enum16Type enum16Type, object value);

        void Write(UInt16Type uInt16Type, object value);

        void Write(NothingType nothingType, object value);

        void Write(Int16Type int16Type, object value);

        void Write(UInt8Type uInt8Type, object value);

        void Write(FixedStringType fixedStringType, object value);

        void Write(Int64Type int64Type, object value);

        void Write(UInt64Type uInt64Type, object value);

        void Write(ArrayType arrayType, object value);

        void Write(DateTime64Type dateTime64Type, object value);

        void Write(NullableType nullableType, object value);

        void Write(IPv6Type pv6Type, object value);

        void Write(TupleType tupleType, object value);

        void Write(NestedType nestedType, object value);

        void Write(DateType dateTimeType, object value);

        void Write(DateTimeType dateTimeType, object value);

        void Write(DecimalType decimalType, object value);

        void Write(EnumType enumType, object value);
    }
}
