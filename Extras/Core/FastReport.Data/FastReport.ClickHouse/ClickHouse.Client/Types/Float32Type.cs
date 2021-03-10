using System;

namespace ClickHouse.Client.Types
{
    internal class Float32Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(float);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Float32;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
