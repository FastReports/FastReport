using System;
using ClickHouse.Client.Types.Grammar;
using NodaTime;

namespace ClickHouse.Client.Types
{
    internal class DateTimeType : AbstractDateTimeType
    {
        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.DateTime;

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            var timeZoneName = node.ChildNodes.Count > 0 ? node.SingleChild.Value.Trim('\'') : string.Empty;
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneName) ?? DateTimeZone.Utc;

            return new DateTimeType { TimeZone = timeZone };
        }

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
