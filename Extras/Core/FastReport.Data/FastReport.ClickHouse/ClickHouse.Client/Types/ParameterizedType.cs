using System;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal abstract class ParameterizedType : ClickHouseType
    {
        public virtual string Name => TypeCode.ToString();

        public abstract ParameterizedType Parse(SyntaxTreeNode typeName, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc);
    }
}
