using System;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal class Decimal128Type : DecimalType
    {
        public Decimal128Type()
        {
            Precision = 38;
        }

        public override int Size => 16;

        public override string Name => "Decimal128";

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            return new Decimal128Type
            {
                Scale = int.Parse(node.SingleChild.Value),
            };
        }

        public override string ToString() => $"{Name}({Scale})";
    }
}
