using System;
using ClickHouse.Client.Types.Grammar;
using NodaTime;

namespace ClickHouse.Client.Types
{
    internal class DateTime64Type : AbstractDateTimeType
    {
        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.DateTime64;

        public override Type FrameworkType => typeof(DateTime);

        public int Scale { get; set; }

        public override string ToString() => TimeZone == null ? $"DateTime64({Scale})" : $"DateTime64({Scale}, {TimeZone.Id})";

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            var scale = int.Parse(node.ChildNodes[0].Value);
            var timeZone = DateTimeZone.Utc;
            if (node.ChildNodes.Count > 1)
            {
                var timeZoneName = node.ChildNodes[1].Value.Trim('\'');
                timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneName) ?? DateTimeZone.Utc;
            }

            return new DateTime64Type
            {
                TimeZone = timeZone,
                Scale = scale,
            };
        }

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
