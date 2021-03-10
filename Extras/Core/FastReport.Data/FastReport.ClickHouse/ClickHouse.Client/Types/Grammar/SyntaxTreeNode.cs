using System;
using System.Collections.Generic;
using System.Text;

namespace ClickHouse.Client.Types.Grammar
{
    public class SyntaxTreeNode
    {
        public string Value { get; set; }

        public IList<SyntaxTreeNode> ChildNodes { get; } = new List<SyntaxTreeNode>();

        public SyntaxTreeNode SingleChild => ChildNodes.Count == 1 ? ChildNodes[0] : throw new ArgumentOutOfRangeException();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Value);
            if (ChildNodes.Count > 0)
            {
                builder.Append("(");
                builder.Append(string.Join(", ", ChildNodes));
                builder.Append(")");
            }
            return builder.ToString();
        }
    }
}
