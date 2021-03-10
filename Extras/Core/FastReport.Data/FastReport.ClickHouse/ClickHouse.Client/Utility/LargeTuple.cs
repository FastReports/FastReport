using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ClickHouse.Client.Utility
{
    internal class LargeTuple : ITuple
    {
        private readonly object[] items;

        public LargeTuple(params object[] items)
        {
            this.items = items;
        }

        public object this[int index] => items[index];

        public int Length => items.Length;

        public int Count => items.Length;

        public IEnumerator<object> GetEnumerator() => items.Cast<object>().GetEnumerator();
    }
}
