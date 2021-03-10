using System;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal class LowCardinalityType : ParameterizedType
    {
        public ClickHouseType UnderlyingType { get; set; }

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.LowCardinality;

        public override string Name => "LowCardinality";

        public override Type FrameworkType => UnderlyingType.FrameworkType;

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            return new LowCardinalityType
            {
                UnderlyingType = parseClickHouseTypeFunc(node.SingleChild),
            };
        }

        public override string ToString() => $"{Name}({UnderlyingType})";
    }
}
