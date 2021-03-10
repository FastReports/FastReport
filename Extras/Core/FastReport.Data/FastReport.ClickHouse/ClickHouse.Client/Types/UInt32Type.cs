using System;

namespace ClickHouse.Client.Types
{
    internal class UInt32Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(uint);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.UInt32;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
