using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace FastReport.Utils
{
    /// <summary>
    /// Represents a xml property.
    /// </summary>
    public struct XmlProperty
    {
        private readonly string key;
        private readonly string value;

        /// <summary>
        /// Represents a property key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Represents a property value.
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        private XmlProperty(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Creates new property and assigns value
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public static XmlProperty Create(string key, string value)
        {
            return new XmlProperty(key, value);
        }
    }

    /// <summary>
    /// Represents a xml node.
    /// </summary>
    public class XmlItem : IDisposable
    {
        private List<XmlItem> items;
        private XmlItem parent;
        private string name;
        private string value;
        private XmlProperty[] properties;
        
        /// <summary>
        /// Gets a number of children in this node.
        /// </summary>
        public int Count
        {
            get { return items == null ? 0 : items.Count; }
        }

        /// <summary>
        /// Gets a list of children in this node.
        /// </summary>
        public List<XmlItem> Items
        {
            get
            {
                if (items == null)
                    items = new List<XmlItem>();
                return items;
            }
        }

        /// <summary>
        /// Gets a child node with specified index.
        /// </summary>
        /// <param name="index">Index of node.</param>
        /// <returns>The node with specified index.</returns>
        public XmlItem this[int index]
        {
            get { return Items[index]; }
        }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        /// <remarks>
        /// This property will return "Node" for a node like <c>&lt;Node Text="" Left="0"/&gt;</c>
        /// </remarks>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets a list of properties in this node.
        /// </summary>
        public XmlProperty[] Properties
        {
            get
            {
                if (properties == null)
                    properties = new XmlProperty[0];
                return properties;
            }
            set
            {
                properties = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent for this node.
        /// </summary>
        public XmlItem Parent
        {
            get { return parent; }
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.Items.Remove(this);
                    if (value != null)
                        value.Items.Add(this);
                }
                parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <remarks>
        /// This property will return "ABC" for a node like <c>&lt;Node&gt;ABC&lt;/Node&gt;</c>
        /// </remarks>
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets the root node which owns this node.
        /// </summary>
        public XmlItem Root
        {
            get
            {
                XmlItem result = this;
                while (result.Parent != null)
                {
                    result = result.Parent;
                }
                return result;
            }
        }

        /// <summary>
        /// Clears the child nodes of this node.
        /// </summary>
        public void Clear()
        {
            if (items != null)
            {
                items.Clear();
                /*        while (Items.Count > 0)
                        {
                          Items[0].Dispose();
                        }  */
                items = null;
            }
        }

        /// <summary>
        /// Adds a new child node to this node.
        /// </summary>
        /// <returns>The new child node.</returns>
        public XmlItem Add()
        {
            XmlItem result = new XmlItem();
            AddItem(result);
            return result;
        }

        /// <summary>
        /// Adds a specified node to this node.
        /// </summary>
        /// <param name="item">The node to add.</param>
        public void AddItem(XmlItem item)
        {
            item.Parent = this;
        }

        /// <summary>
        /// Inserts a specified node to this node.
        /// </summary>
        /// <param name="index">Position to insert.</param>
        /// <param name="item">Node to insert.</param>
        public void InsertItem(int index, XmlItem item)
        {
            AddItem(item);
            Items.RemoveAt(Count - 1);
            Items.Insert(index, item);
        }

        /// <summary>
        /// Finds the node with specified name.
        /// </summary>
        /// <param name="name">The name of node to find.</param>
        /// <returns>The node with specified name, if found; <b>null</b> otherwise.</returns>
        public int Find(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (String.Compare(Items[i].Name, name, true) == 0)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Finds the node with specified name.
        /// </summary>
        /// <param name="name">The name of node to find.</param>
        /// <returns>The node with specified name, if found; the new node otherwise.</returns>
        /// <remarks>
        /// This method adds the node with specified name to the child nodes if it cannot find the node.
        /// Do not dispose items, which has been created by this method
        /// </remarks>
        public XmlItem FindItem(string name)
        {
            XmlItem result = null;
            int i = Find(name);
            if (i == -1)
            {
                result = Add();
                result.Name = name;
            }
            else
                result = Items[i];
            return result;
        }

        /// <summary>
        /// Gets the index of specified node in the child nodes list.
        /// </summary>
        /// <param name="item">The node to find.</param>
        /// <returns>Zero-based index of node, if found; <b>-1</b> otherwise.</returns>
        public int IndexOf(XmlItem item)
        {
            return Items.IndexOf(item);
        }

        /// <summary>
        /// Gets a property with specified name.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <returns>The value of property, if found; empty string otherwise.</returns>
        /// <remarks>
        /// This property will return "0" when you request the "Left" property for a node 
        /// like <c>&lt;Node Text="" Left="0"/&gt;</c>
        /// </remarks>
        public string GetProp(string key)
        {
            return GetProp(key, true);
        }

        internal string GetProp(string key, bool convertFromXml)
        {
            if (properties == null || properties.Length == 0)
                return "";

            // property key should be trimmed
            key = key.Trim();

            foreach (XmlProperty kv in properties)
                if (kv.Key == key)
                    return kv.Value;

            return "";
        }

        internal void WriteProps(FastString sb)
        {
            if (properties == null || properties.Length == 0)
                return;

            sb.Append(" ");
            foreach (XmlProperty kv in properties)
            {
                //if (string.IsNullOrWhiteSpace(kv.Key))
                if (String.IsNullOrEmpty(kv.Key) || kv.Key.Trim().Length == 0)
                    continue;

                sb.Append(kv.Key);
                sb.Append("=\"");
                sb.Append(Converter.ToXml(kv.Value));
                sb.Append("\" ");
            }
            sb.Length--;
        }

        /// <summary>
        /// Removes all properties.
        /// </summary>
        public void ClearProps()
        {
            properties = null;
        }

        internal void CopyPropsTo(XmlItem item)
        {
            if (properties == null)
            {
                item.properties = null;
                return;
            }

            item.properties = (XmlProperty[])properties.Clone();
        }

        internal bool IsNullOrEmptyProps()
        {
            return properties == null || properties.Length == 0;
        }

        /// <summary>
        /// Sets the value for a specified property.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <param name="value">Value to set.</param>
        /// <remarks>
        /// For example, you have a node like <c>&lt;Node Text="" Left="0"/&gt;</c>. When you set the
        /// "Text" property to "test", the node will be <c>&lt;Node Text="test" Left="0"/&gt;</c>.
        /// If property with specified name is not exist, it will be added.
        /// </remarks>
        public void SetProp(string key, string value)
        {
            // property key should be trimmed
            key = key.Trim();

            if (properties == null)
            {
                properties = new XmlProperty[1];
                properties[0] = XmlProperty.Create(key, value);
                return;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Key == key)
                {
                    properties[i] = XmlProperty.Create(key, value);
                    return;
                }
            }

            Array.Resize<XmlProperty>(ref properties, properties.Length + 1);
            properties[properties.Length - 1] = XmlProperty.Create(key, value);
        }

        /// <summary>
        /// Removes a property with specified name.
        /// </summary>
        /// <param name="key">The property name.</param>
        /// <returns>Returns true if property is removed, false otherwise.</returns>
        public bool RemoveProp(string key)
        {
            if (properties == null || properties.Length == 0)
                return false;

            // property key should be trimmed
            key = key.Trim();

            if (properties.Length == 1 && properties[0].Key == key)
            {
                properties = null;
                return true;
            }
            
            if (properties[properties.Length - 1].Key == key)
            {
                Array.Resize<XmlProperty>(ref properties, properties.Length - 1);
                return true;
            }

            int target = -1;

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Key == key)
                {
                    target = i;
                    break;
                }
            }

            if (target != -1)
            {
                for (int i = target; i < properties.Length - 1; i++)
                {
                    properties[i] = properties[i + 1];
                }

                Array.Resize<XmlProperty>(ref properties, properties.Length - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Disposes the node and all its children.
        /// </summary>
        public void Dispose()
        {
            Clear();
            Parent = null;
        }

        /// <summary>
        /// Initializes a new instance of the <b>XmlItem</b> class with default settings.
        /// </summary>
        public XmlItem()
        {
            name = "";
            value = "";
        }
    }


    /// <summary>
    /// Represents a xml document that contains the root xml node.
    /// </summary>
    /// <remarks>
    /// Use <b>Load</b> and <b>Save</b> methods to load/save the document. To access the root node
    /// of the document, use the <see cref="Root"/> property.
    /// </remarks>
    public class XmlDocument : IDisposable
    {
        private bool autoIndent;
        private bool writeHeader;
        private XmlItem root;

        /// <summary>
        /// Gets or sets a value indicating whether is necessary to indent the document
        /// when saving it to a file/stream.
        /// </summary>
        public bool AutoIndent
        {
            get { return autoIndent; }
            set { autoIndent = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is necessary to add xml header.
        /// </summary>
        public bool WriteHeader
        {
            get { return writeHeader; }
            set { writeHeader = value; }
        }

        /// <summary>
        /// Gets the root node of the document.
        /// </summary>
        public XmlItem Root
        {
            get { return root; }
        }

        /// <summary>
        /// Clears the document.
        /// </summary>
        public void Clear()
        {
            root.Clear();
        }

        /// <summary>
        /// Saves the document to a stream.
        /// </summary>
        /// <param name="stream">Stream to save to.</param>
        public void Save(Stream stream)
        {
            XmlWriter wr = new XmlWriter(stream);
            wr.AutoIndent = autoIndent;
            wr.IsWriteHeader = WriteHeader;
            wr.Write(root);
        }

        /// <summary>
        /// Saves the document to a string.
        /// </summary>
        /// <param name="textWriter">Writer to save to.</param>
        public void Save(TextWriter textWriter)
        {
            XmlWriter wr = new XmlWriter(textWriter);
            wr.AutoIndent = autoIndent;
            wr.IsWriteHeader = WriteHeader;
            wr.Write(root);
        }

        /// <summary>
        /// Loads the document from a stream.
        /// </summary>
        /// <param name="stream">Stream to load from.</param>
        public void Load(Stream stream)
        {
            XmlReader rd = new XmlReader(stream);
            root.Clear();
            rd.Read(root);
        }

        /// <summary>
        /// Saves the document to a file.
        /// </summary>
        /// <param name="fileName">The name of file to save to.</param>
        public void Save(string fileName)
        {
            FileStream s = new FileStream(fileName, FileMode.Create);
            Save(s);
            s.Close();
        }

        /// <summary>
        /// Loads the document from a file.
        /// </summary>
        /// <param name="fileName">The name of file to load from.</param>
        public void Load(string fileName)
        {
            FileStream s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(s);
            s.Close();
        }

        /// <summary>
        /// Disposes resources used by the document.
        /// </summary>
        public void Dispose()
        {
            root.Dispose();
        }

        /// <summary>
        /// Initializes a new instance of the <b>XmlDocument</b> class with default settings.
        /// </summary>
        public XmlDocument()
        {
            root = new XmlItem();
            writeHeader = true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            using (TextWriter tw = new StringWriter())
            {
                this.Save(tw);
                tw.Flush();
                return tw.ToString();
            }
        }
    }

    internal class XmlReader
    {
        private StreamReader reader;
        private Stream stream;
        private string lastName;
        private enum ReadState { FindLeft, FindRight, FindComment, FindCloseItem, Done }
        private enum ItemState { Begin, End, Complete }
        private int symbolInBuffer;
        Dictionary<string, string> stringPool;
        private ItemState ReadItem(XmlItem item)
        {
            FastString builder;
            if (Config.IsStringOptimization)
                builder = new FastStringWithPool(stringPool);
            else
                builder = new FastString();
            ReadState state = ReadState.FindLeft;
            int comment = 0;
            int i = 0;
            //string tempAttrName = null;
            //int lc = -1;
            int c = -1;

            // find <
            c = readNextSymbol();
            while (c != -1)
            {
                if (c == '<')
                    break;
                c = readNextSymbol();
            }

            //while not end
            while (state != ReadState.Done && c != -1)
            {
                // find name or comment;
                c = readNextSymbol();
                i = 0;
                while (c != -1)
                {
                    if (i <= comment)
                    {
                        switch (comment)
                        {
                            case 0: if (c == '!') comment++; break;
                            case 1: if (c == '-') comment++; break;
                            case 2: if (c == '-') state = ReadState.FindComment; break;
                            default: comment = -1; break;
                        }
                        if (state == ReadState.FindComment) break;
                    }
                    i++;
                    switch (c)
                    {
                        case '>': state = ReadState.Done; break; //Found name
                        case ' ': state = ReadState.FindRight; break; //Found name
                        case '<': RaiseException(); break;
                        default: builder.Append((char)c); break;
                    }
                    if (state != ReadState.FindLeft) break;
                    c = readNextSymbol();
                }
                switch (state)
                {
                    case ReadState.FindComment:
                        comment = 0;
                        while (c != -1)
                        {
                            c = readNextSymbol();
                            if (comment > 1)
                            {
                                if (c == '>')
                                {
                                    state = ReadState.FindLeft;
                                    break;
                                }
                            }
                            else
                            {
                                if (c == '-')
                                    comment++;
                                else
                                    comment = 0;
                            }
                        }
                        comment = 0;
                        builder.Length = 0;
                        while (c != -1)
                        {
                            if (c == '<')
                                break;
                            c = readNextSymbol();
                        }
                        break;
                    case ReadState.Done:
                        string result = builder.ToString();
                        if (result[0] == '/')
                        {
                            item.Name = result.Substring(1);
                            return ItemState.End;
                        }
                        if (result[result.Length - 1] == '/')
                        {
                            item.Name = result.Substring(0, result.Length - 1);
                            return ItemState.Complete;
                        }
                        item.Name = result;
                        return ItemState.Begin;
                    case ReadState.FindRight:
                        if (builder[0] == '/')
                        {
                            builder.Remove(0, 1);
                            item.Name = builder.ToString();
                            return ItemState.End;
                        }
                        item.Name = builder.ToString();
                        builder.Length = 0;
                        while (c != -1 && c != '>')
                        {
                            c = readNextSymbol();
                            while (c != -1)
                            {
                                if (c == ' ')
                                {
                                    builder.Length = 0;
                                    c = readNextSymbol();
                                    continue;
                                }
                                if (c == '=' || c == '>')
                                    break;
                                builder.Append((char)c);
                                c = readNextSymbol();
                            }
                            if (c == '>')
                            {

                                if (builder.Length > 0 && builder[builder.Length - 1] == '/')
                                    return ItemState.Complete;
                                return ItemState.Begin;
                            }
                            c = readNextSymbol();
                            if (c != '"')
                                continue;
                            string attrName = builder.ToString();
                            builder.Length = 0;
                            while (c != -1)
                            {
                                c = readNextSymbol();
                                if (c == '"')
                                    break;
                                builder.Append((char)c);
                            }
                            item.SetProp(attrName, Converter.FromXml(builder.ToString()));
                            builder.Length = 0;
                        }
                        break;
                }
            }


            //just for errors
            return ItemState.Begin;
        }

        private int readNextSymbol()
        {
            if (symbolInBuffer != -1)
            {
                int temp = symbolInBuffer;
                symbolInBuffer = -1;
                return temp;
            }
            return reader.Read();
        }

        private bool ReadValue(XmlItem item)
        {
            FastString builder;
            if (Config.IsStringOptimization)
                builder = new FastStringWithPool(stringPool);
            else
                builder = new FastString();
            ReadState state = ReadState.FindLeft;
            string lastName = "</" + this.lastName + ">";
            int lastNameLength = lastName.Length;

            do
            {
                int c = reader.Read();
                if (c == -1)
                    RaiseException();

                builder.Append((char)c);
                if (state == ReadState.FindLeft)
                {
                    if (c == '<')
                    {
                        symbolInBuffer = '<';
                        return false;
                    }
                    else if (c != ' ' && c != '\r' && c != '\n' && c != '\t')
                        state = ReadState.FindCloseItem;
                }
                else if (state == ReadState.FindCloseItem)
                {
                    if (builder.Length >= lastNameLength)
                    {
                        bool match = true;
                        for (int j = 0; j < lastNameLength; j++)
                        {
                            if (builder[builder.Length - lastNameLength + j] != lastName[j])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            builder.Length -= lastNameLength;
                            item.Value = Converter.FromXml(builder.ToString());
                            return true;
                        }
                    }
                }
            }
            while (true);
        }

        private bool DoRead(XmlItem rootItem)
        {
            ItemState itemState = ReadItem(rootItem);
            lastName = rootItem.Name;

            if (itemState == ItemState.End)
                return true;
            else if (itemState == ItemState.Complete)
                return false;

            if (ReadValue(rootItem))
                return false;

            bool done = false;
            do
            {
                XmlItem childItem = new XmlItem();
                done = DoRead(childItem);
                if (!done)
                    rootItem.AddItem(childItem);
                else
                    childItem.Dispose();
            }
            while (!done);

            if (lastName != "" && String.Compare(lastName, rootItem.Name, true) != 0)
                RaiseException();

            return false;
        }

        private void RaiseException()
        {
            throw new FileFormatException();
        }

        private void ReadHeader()
        {
            using (XmlItem item = new XmlItem())
            {
                ReadItem(item);
                if (item.Name.IndexOf("?xml") != 0)
                    RaiseException();
            }
        }

        public void Read(XmlItem item)
        {
            ReadHeader();
            DoRead(item);
        }

        public XmlReader(Stream stream)
        {
            this.stream = stream;
            reader = new StreamReader(this.stream, Encoding.UTF8);
            if (Config.IsStringOptimization)
                stringPool = new Dictionary<string, string>();
            lastName = "";
            symbolInBuffer = -1;
        }
    }


    internal class XmlWriter
    {
        private bool autoIndent;
        private bool isWriteHeader;
        //private Stream FStream;
        private TextWriter writer;

        public bool AutoIndent
        {
            get { return autoIndent; }
            set { autoIndent = value; }
        }

        public bool IsWriteHeader
        {
            get { return isWriteHeader; }
            set { isWriteHeader = value; }
        }

        private void WriteLn(string s)
        {
            if (!autoIndent)
                writer.Write(s);
            else
                writer.Write(s + "\r\n");
        }

        private string Dup(int num)
        {
            string s = "";
            return s.PadLeft(num);
        }

        private void WriteItem(XmlItem item, int level)
        {
            FastString sb = new FastString();


            // start
            if (autoIndent)
                sb.Append(Dup(level));
            sb.Append("<");
            sb.Append(item.Name);

            // text

            item.WriteProps(sb);

            // end
            if (item.Count == 0 && item.Value == "")
                sb.Append("/>");
            else
                sb.Append(">");

            // value
            if (item.Count == 0 && item.Value != "")
            {
                sb.Append(Converter.ToXml(item.Value,false));
                sb.Append("</");
                sb.Append(item.Name);
                sb.Append(">");
            }
            WriteLn(sb.ToString());
        }

        private void DoWrite(XmlItem rootItem, int level)
        {
            if (!autoIndent)
                level = 0;

            WriteItem(rootItem, level);
            for (int i = 0; i < rootItem.Count; i++)
                DoWrite(rootItem[i], level + 2);

            if (rootItem.Count > 0)
            {
                if (!autoIndent)
                    WriteLn("</" + rootItem.Name + ">");
                else
                    WriteLn(Dup(level) + "</" + rootItem.Name + ">");
            }
        }

        private void WriteHeader()
        {
            WriteLn("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        public void Write(XmlItem rootItem)
        {
            if (IsWriteHeader)
                WriteHeader();
            DoWrite(rootItem, 0);
            writer.Flush();
        }

        public XmlWriter(Stream stream)
        {
            //FStream = stream;
            writer = new StreamWriter(stream, Encoding.UTF8);
            isWriteHeader = true;
        }

        public XmlWriter(TextWriter textWriter)
        {
            //FStream = null;
            writer = textWriter;
            isWriteHeader = true;
        }
    }
}