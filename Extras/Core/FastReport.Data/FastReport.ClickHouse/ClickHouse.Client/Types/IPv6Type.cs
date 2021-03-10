using System;
using System.Net;

namespace ClickHouse.Client.Types
{
    internal class IPv6Type : ClickHouseType
    {
        public override Type FrameworkType => typeof(IPAddress);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.IPv6;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
