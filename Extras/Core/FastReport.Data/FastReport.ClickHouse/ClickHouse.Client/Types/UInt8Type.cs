using System;

namespace ClickHouse.Client.Types
{
    internal class UInt8Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(byte);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.UInt8;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
