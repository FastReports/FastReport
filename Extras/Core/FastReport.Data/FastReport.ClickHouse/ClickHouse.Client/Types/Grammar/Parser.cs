using System.Collections.Generic;
using System.Linq;

namespace ClickHouse.Client.Types.Grammar
{
    public static class Parser
    {
        public static SyntaxTreeNode Parse(string input)
        {
            var tokens = Tokenizer.GetTokens(input).ToList();
            var stack = new Stack<SyntaxTreeNode>();
            SyntaxTreeNode current = null;

            foreach (var token in tokens)
            {
                switch (token)
                {
                    case "(":
                        stack.Push(current);
                        break;
                    case ",":
                        stack.Peek().ChildNodes.Add(current);
                        break;
                    case ")":
                        stack.Peek().ChildNodes.Add(current);
                        current = stack.Pop();
                        break;
                    default:
                        current = new SyntaxTreeNode { Value = token };
                        break;
                }
            }
            return current;
        }
    }
}
