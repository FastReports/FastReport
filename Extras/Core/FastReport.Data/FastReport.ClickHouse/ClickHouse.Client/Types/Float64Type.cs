using System;

namespace ClickHouse.Client.Types
{
    internal class Float64Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(double);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Float64;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
