// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        public class ObjectCollection : IList
        {
            private readonly ComboBox owner;
            private ArrayList innerList;
            private IComparer comparer;

            public ObjectCollection(ComboBox owner)
            {
                this.owner = owner;
            }

            private IComparer Comparer
            {
                get
                {
                    if (comparer == null)
                    {
                        comparer = new ItemComparer(owner);
                    }
                    return comparer;
                }
            }

            private ArrayList InnerList
            {
                get
                {
                    if (innerList == null)
                    {
                        innerList = new ArrayList();
                    }
                    return innerList;
                }
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    return InnerList.Count;
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
            ///  Adds an item to the combo box. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
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
                    InnerList.Add(item);
                }
                else
                {
                    index = InnerList.BinarySearch(item, Comparer);
                    if (index < 0)
                    {
                        index = ~index; // getting the index of the first element that is larger than the search value
                    }

                    InnerList.Insert(index, item);
                }
                bool successful = false;

                try
                {
                    if (owner.sorted)
                    {
                    }
                    else
                    {
                        index = InnerList.Count - 1;
                    }
                    successful = true;
                }
                finally
                {
                    if (!successful)
                    {
                        InnerList.Remove(item);
                    }
                }

                return index;
            }

            int IList.Add(object item)
            {
                return Add(item);
            }

            /// <summary>
            ///  Retrieves the item with the specified index.
            /// </summary>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual object this[int index]
            {
                get
                {
                    if (index < 0 || index >= InnerList.Count)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    return InnerList[index];
                }
                set
                {
                    SetItemInternal(index, value);
                }
            }

            public void AddRange(object[] items)
            {
                try
                {
                    AddRangeInternal(items);
                }
                finally
                {
                }
            }

            internal void AddRangeInternal(IList items)
            {

                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                foreach (object item in items)
                {
                    // adding items one-by-one for performance (especially for sorted combobox)
                    // we can not rely on ArrayList.Sort since its worst case complexity is n*n
                    // AddInternal is based on BinarySearch and ensures n*log(n) complexity
                    AddInternal(item);
                }
            }

            /// <summary>
            ///  Removes all items from the ComboBox.
            /// </summary>
            public void Clear()
            {
                ClearInternal();
            }

            internal void ClearInternal()
            {

                InnerList.Clear();
                owner.selectedIndex = -1;
            }

            public bool Contains(object value)
            {
                return IndexOf(value) != -1;
            }

            /// <summary>
            ///  Copies the ComboBox Items collection to a destination array.
            /// </summary>
            public void CopyTo(object[] destination, int arrayIndex)
            {
                InnerList.CopyTo(destination, arrayIndex);
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                InnerList.CopyTo(destination, index);
            }

            /// <summary>
            ///  Returns an enumerator for the ComboBox Items collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                return InnerList.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerList.IndexOf(value);
            }

            /// <summary>
            ///  Adds an item to the combo box. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
            ///  A SystemException occurs if there is insufficient space available to
            ///  store the new item.
            /// </summary>
            public void Insert(int index, object item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (index < 0 || index > InnerList.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // If the combo box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (owner.sorted)
                {
                    Add(item);
                }
                else
                {
                    InnerList.Insert(index, item);
                }
            }

            /// <summary>
            ///  Removes an item from the ComboBox at the given index.
            /// </summary>
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= InnerList.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                InnerList.RemoveAt(index);
                if (index < owner.selectedIndex)
                {
                    owner.selectedIndex--;
                }
            }

            /// <summary>
            ///  Removes the given item from the ComboBox, provided that it is
            ///  actually in the list.
            /// </summary>
            public void Remove(object value)
            {

                int index = InnerList.IndexOf(value);

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            internal void SetItemInternal(int index, object value)
            {
                if (index < 0 || index >= InnerList.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                InnerList[index] = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        private sealed class ItemComparer : IComparer
        {
            private readonly ComboBox comboBox;

            public ItemComparer(ComboBox comboBox)
            {
                this.comboBox = comboBox;
            }

            public int Compare(object item1, object item2)
            {
                if (item1 == null)
                {
                    if (item2 == null)
                    {
                        return 0; //both null, then they are equal
                    }

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                {
                    return 1; //item2 is null, so item 1 is greater
                }

                string itemName1 = comboBox.GetItemText(item1);
                string itemName2 = comboBox.GetItemText(item2);

                CompareInfo compInfo = CultureInfo.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }
    }
}
