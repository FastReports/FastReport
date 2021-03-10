using System;

namespace ClickHouse.Client.Types
{
    internal class Int32Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(int);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Int32;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
