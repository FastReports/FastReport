using System;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal class FixedStringType : ParameterizedType
    {
        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.FixedString;

        public int Length { get; set; }

        public override Type FrameworkType => typeof(string);

        public override string Name => "FixedString";

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            return new FixedStringType
            {
                Length = int.Parse(node.SingleChild.Value),
            };
        }

        public override string ToString() => $"FixedString{Length}";
    }
}
