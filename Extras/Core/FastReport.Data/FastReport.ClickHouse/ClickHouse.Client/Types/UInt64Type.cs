using System;

namespace ClickHouse.Client.Types
{
    internal class UInt64Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(ulong);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.UInt64;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
