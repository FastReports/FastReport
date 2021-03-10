using System;

namespace ClickHouse.Client.Types
{
    internal class Int16Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(short);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Int16;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
