using System;
using System.Collections.Generic;
using System.Linq;
using ClickHouse.Client.Types.Grammar;

namespace ClickHouse.Client.Types
{
    internal class EnumType : ParameterizedType
    {
        private Dictionary<string, int> values = new Dictionary<string, int>();

        public override string Name => "Enum";

        public override Type FrameworkType => typeof(string);

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Enum8;

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            var parameters = node.ChildNodes
                .Select(cn => cn.Value)
                .Select(p => p.Split('='))
                .ToDictionary(kvp => kvp[0].Trim().Trim('\''), kvp => Convert.ToInt32(kvp[1].Trim()));

            switch (node.Value)
            {
                case "Enum":
                case "Enum8":
                    return new Enum8Type { values = parameters };
                case "Enum16":
                    return new Enum16Type { values = parameters };
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public int Lookup(string key) => values[key];

        public string Lookup(int value) => values.SingleOrDefault(kvp => kvp.Value == value).Key ?? throw new KeyNotFoundException();

        public override string ToString() => $"{Name}({string.Join(",", values.Select(kvp => kvp.Key + "=" + kvp.Value))}";

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
