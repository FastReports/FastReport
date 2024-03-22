// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2007 Novell, Inc.
//

// Modified by: Alexander Tzyganenko

using System;
using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    public class BindingSource : Component, IList, ICollection, IEnumerable
    {
        IList list;
        object datasource;

        public object DataSource
        {
            get { return datasource; }
            set
            {
                if (datasource != value)
                {
                    datasource = value;
                    ResetList();
                }
            }
        }

        public bool IsFixedSize => list.IsFixedSize;

        public bool IsReadOnly => list.IsReadOnly;

        public int Count => list.Count;

        public bool IsSynchronized => list.IsSynchronized;

        public object SyncRoot => list.SyncRoot;

        public object this[int index] { get => list[index]; set => list[index] = value; }

        IList GetListFromEnumerable(IEnumerable enumerable)
        {
          IList l;

          IEnumerator e = enumerable.GetEnumerator();

          if (enumerable is string)
          {
            /* special case this.. seems to be the only one .net special cases? */
            l = new BindingList<char>();
          }
          else
          {
            /* try to figure out the type based on
             * the first element, if there is
             * one */
            object first = null;
            if (e.MoveNext())
            {
              first = e.Current;
            }

            if (first == null)
            {
              return null;
            }
            else
            {
              Type t = typeof(BindingList<>).MakeGenericType(new Type[] { first.GetType() });
              l = (IList)Activator.CreateInstance(t);
            }
          }

          e.Reset();
          while (e.MoveNext())
          {
            l.Add(e.Current);
          }

          return l;
        }

        void ResetList()
        {
          IList l;
          object source = ListBindingHelper.GetList(datasource, null);

          // 
          // If original source is null, then create a new object list
          // Otherwise, try to infer the list item type
          //

          if (datasource == null)
          {
            l = new BindingList<object>();
          }
          else if (source == null)
          {
            //Infer type based on datasource and datamember,
            // where datasource is an empty IEnumerable
            // and need to find out the datamember type

            Type property_type = ListBindingHelper.GetListItemProperties(datasource)[""].PropertyType;
            Type t = typeof(BindingList<>).MakeGenericType(new Type[] { property_type });
            l = (IList)Activator.CreateInstance(t);
          }
          else if (source is IList)
          {
            l = (IList)source;
          }
          else if (source is IEnumerable)
          {
            IList new_list = GetListFromEnumerable((IEnumerable)source);
            l = new_list == null ? list : new_list;
          }
          else if (source is Type)
          {
            Type t = typeof(BindingList<>).MakeGenericType(new Type[] { (Type)source });
            l = (IList)Activator.CreateInstance(t);
          }
          else
          {
            Type t = typeof(BindingList<>).MakeGenericType(new Type[] { source.GetType() });
            l = (IList)Activator.CreateInstance(t);
            l.Add(source);
          }

          list = l;
        }

        public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
          return ListBindingHelper.GetListItemProperties(list, listAccessors);
        }

        public int Add(object value) => list.Add(value);

        public void Clear() => list.Clear();

        public bool Contains(object value) => list.Contains(value);

        public int IndexOf(object value) => list.IndexOf(value);

        public void Insert(int index, object value) => list.Insert(index, value);

        public void Remove(object value) => list.Remove(value);

        public void RemoveAt(int index) => list.RemoveAt(index);

        public void CopyTo(Array array, int index) => list.CopyTo(array, index);

        public IEnumerator GetEnumerator() => list.GetEnumerator();
    }
}
