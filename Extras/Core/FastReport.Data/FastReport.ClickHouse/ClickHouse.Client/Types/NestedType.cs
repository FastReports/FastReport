using System;
using System.Linq;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal class NestedType : TupleType
    {
        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Nested;

        public override Type FrameworkType => base.FrameworkType.MakeArrayType();

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            return new NestedType
            {
                UnderlyingTypes = node.ChildNodes.Select(parseClickHouseTypeFunc).ToArray(),
            };
        }
    }
}
