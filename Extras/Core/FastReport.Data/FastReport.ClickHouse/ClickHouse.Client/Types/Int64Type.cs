using System;

namespace ClickHouse.Client.Types
{
    internal class Int64Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(long);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Int64;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
