// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        public class ObjectCollection : IList
        {
            private readonly ListBox owner;
            private ItemArray items;

            public ObjectCollection(ListBox owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Initializes a new instance of ListBox.ObjectCollection based on another ListBox.ObjectCollection.
            /// </summary>
            public ObjectCollection(ListBox owner, ObjectCollection value)
            {
                this.owner = owner;
                AddRange(value);
            }

            /// <summary>
            ///  Initializes a new instance of ListBox.ObjectCollection containing any array of objects.
            /// </summary>
            public ObjectCollection(ListBox owner, object[] value)
            {
                this.owner = owner;
                AddRange(value);
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    return InnerArray.GetCount(0);
                }
            }

            /// <summary>
            ///  Internal access to the actual data store.
            /// </summary>
            internal ItemArray InnerArray
            {
                get
                {
                    if (items == null)
                    {
                        items = new ItemArray(owner);
                    }
                    return items;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Adds an item to the List box. For an unsorted List box, the item is
            ///  added to the end of the existing list of items. For a sorted List box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the List box.
            ///  A SystemException occurs if there is insufficient space available to
            ///  store the new item.
            /// </summary>
            public int Add(object item)
            {
                int index = AddInternal(item);
                return index;
            }

            private int AddInternal(object item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
                int index = -1;
                if (!owner.sorted)
                {
                    InnerArray.Add(item);
                }
                else
                {
                    if (Count > 0)
                    {
                        index = InnerArray.BinarySearch(item);
                        if (index < 0)
                        {
                            index = ~index; // getting the index of the first element that is larger than the search value
                                            //this index will be used for insert
                        }
                    }
                    else
                    {
                        index = 0;
                    }

                    InnerArray.Insert(index, item);
                }
                bool successful = false;

                try
                {
                    if (owner.sorted)
                    {
                    }
                    else
                    {
                        index = Count - 1;
                    }
                    successful = true;
                }
                finally
                {
                    if (!successful)
                    {
                        InnerArray.Remove(item);
                    }
                }

                return index;
            }

            int IList.Add(object item)
            {
                return Add(item);
            }

            public void AddRange(ObjectCollection value)
            {
                AddRangeInternal((ICollection)value);
            }

            public void AddRange(object[] items)
            {
                AddRangeInternal((ICollection)items);
            }

            internal void AddRangeInternal(ICollection items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                try
                {
                    foreach (object item in items)
                    {
                        // adding items one-by-one for performance
                        // not using sort because after the array is sorted index of each newly added item will need to be found
                        // AddInternal is based on BinarySearch and finds index without any additional cost
                        AddInternal(item);
                    }
                }
                finally
                {
                }
            }

            /// <summary>
            ///  Retrieves the item with the specified index.
            /// </summary>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual object this[int index]
            {
                get
                {
                    if (index < 0 || index >= InnerArray.GetCount(0))
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    return InnerArray.GetItem(index, 0);
                }
                set
                {
                    SetItemInternal(index, value);
                }
            }

            /// <summary>
            ///  Removes all items from the ListBox.
            /// </summary>
            public virtual void Clear()
            {
                ClearInternal();
            }

            /// <summary>
            ///  Removes all items from the ListBox.  Bypasses the data source check.
            /// </summary>
            internal void ClearInternal()
            {
                InnerArray.Clear();
            }

            public bool Contains(object value)
            {
                return IndexOf(value) != -1;
            }

            /// <summary>
            ///  Copies the ListBox Items collection to a destination array.
            /// </summary>
            public void CopyTo(object[] destination, int arrayIndex)
            {
                int count = InnerArray.GetCount(0);
                for (int i = 0; i < count; i++)
                {
                    destination[i + arrayIndex] = InnerArray.GetItem(i, 0);
                }
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                int count = InnerArray.GetCount(0);
                for (int i = 0; i < count; i++)
                {
                    destination.SetValue(InnerArray.GetItem(i, 0), i + index);
                }
            }

            /// <summary>
            ///  Returns an enumerator for the ListBox Items collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                return InnerArray.GetEnumerator(0);
            }

            public int IndexOf(object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOf(value, 0);
            }

            internal int IndexOfIdentifier(object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOfIdentifier(value, 0);
            }

            /// <summary>
            ///  Adds an item to the List box. For an unsorted List box, the item is
            ///  added to the end of the existing list of items. For a sorted List box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the List box.
            ///  A SystemException occurs if there is insufficient space available to
            ///  store the new item.
            /// </summary>
            public void Insert(int index, object item)
            {
                if (index < 0 || index > InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                // If the List box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (owner.sorted)
                {
                    Add(item);
                }
                else
                {
                    InnerArray.Insert(index, item);
                }
            }

            /// <summary>
            ///  Removes the given item from the ListBox, provided that it is
            ///  actually in the list.
            /// </summary>
            public void Remove(object value)
            {

                int index = InnerArray.IndexOf(value, 0);

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Removes an item from the ListBox at the given index.
            /// </summary>
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException();
                }

                // Update InnerArray before calling NativeRemoveAt to ensure that when
                // SelectedIndexChanged is raised (by NativeRemoveAt), InnerArray's state matches wrapped LB state.
                InnerArray.RemoveAt(index);
            }

            internal void SetItemInternal(int index, object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (index < 0 || index >= InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException();
                }

                InnerArray.SetItem(index, value);
            }
        } // end ObjectCollection}
    }
}
