using System;

namespace ClickHouse.Client.Types
{
    internal class StringType : ClickHouseType
    {
        public override Type FrameworkType => typeof(string);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.String;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
