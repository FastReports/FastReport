using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace FastReport.Utils
{
    /// <summary>
    /// Specifies the target for the serialize operation.
    /// </summary>
    public enum SerializeTo
    {
        /// <summary>
        /// Serialize to the report file.
        /// </summary>
        Report,

        /// <summary>
        /// Serialize to the preview pages.
        /// </summary>
        Preview,

        /// <summary>
        /// Serialize to the source pages of a preview.
        /// </summary>
        SourcePages,

        /// <summary>
        /// Serialize to the designer's clipboard.
        /// </summary>
        Clipboard,

        /// <summary>
        /// Serialize to the designer's undo/redo buffer.
        /// </summary>
        Undo
    }

    internal class DiffEventArgs
    {
        private object obj;
        private object diffObject;

        public object Object
        {
            get { return obj; }
            set { obj = value; }
        }
        public object DiffObject
        {
            get { return diffObject; }
            set { diffObject = value; }
        }
    }

    internal delegate void DiffEventHandler(object sender, DiffEventArgs e);


    /// <summary>
    /// The writer used to serialize object's properties to a report file.
    /// </summary>
    public class FRWriter : IDisposable
    {
        #region Fields
        private XmlDocument doc;
        private XmlItem root;
        private XmlItem curItem;
        private XmlItem curRoot;
        //private StringBuilder FText;
        private object diffObject;
        private bool saveChildren;
        private bool writeHeader;
        private BlobStore blobStore;
        private SerializeTo serializeTo;
        private Hashtable diffObjects;
        #endregion

        #region Properties
        internal event DiffEventHandler GetDiff;

        internal BlobStore BlobStore
        {
            get { return blobStore; }
            set { blobStore = value; }
        }

        /// <summary>
        /// Gets or sets current xml item name.
        /// </summary>
        public string ItemName
        {
            get { return curItem.Name; }
            set { curItem.Name = value; }
        }

        /// <summary>
        /// Gets or sets target of serialization.
        /// </summary>
        public SerializeTo SerializeTo
        {
            get { return serializeTo; }
            set { serializeTo = value; }
        }

        /// <summary>
        /// Gets the ethalon object to compare with.
        /// </summary>
        public object DiffObject
        {
            get { return diffObject; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether is necessary to serialize child objects.
        /// </summary>
        public bool SaveChildren
        {
            get { return saveChildren; }
            set { saveChildren = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether is necessary to add xml header.
        /// </summary>
        public bool WriteHeader
        {
            get { return writeHeader; }
            set { writeHeader = value; }
        }
        #endregion

        #region Private Methods
        private string PropName(string name)
        {
            return serializeTo == SerializeTo.Preview ? ShortProperties.GetShortName(name) : name;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <remarks>
        /// The object must implement the <see cref="IFRSerializable"/> interface. This method
        /// invokes the <b>Serialize</b> method of the object.
        /// </remarks>
        /// <example>This example demonstrates the use of writer.
        /// <code>
        /// public void Serialize(FRWriter writer)
        /// {
        ///   // get the etalon object. It will be used to write changed properties only.
        ///   Base c = writer.DiffObject as Base;
        /// 
        ///   // write the type name
        ///   writer.ItemName = ClassName;
        /// 
        ///   // write properties
        ///   if (Name != "")
        ///     writer.WriteStr("Name", Name);
        ///   if (Restrictions != c.Restrictions)
        ///     writer.WriteValue("Restrictions", Restrictions);
        ///   
        ///   // write child objects if allowed
        ///   if (writer.SaveChildren)
        ///   {
        ///     foreach (Base child in ChildObjects)
        ///     {
        ///       writer.Write(child);
        ///     }
        ///   }
        /// }
        /// </code>
        /// </example>
        public void Write(IFRSerializable obj)
        {
            Write(obj, null);
        }

        /// <summary>
        /// Serializes the object using specified etalon.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="diff">The etalon object.</param>
        public void Write(IFRSerializable obj, object diff)
        {
            if (obj == null)
                return;
            XmlItem saveCurItem = curItem;
            XmlItem saveCurRoot = curRoot;
            //StringBuilder saveText = FText;
            object saveDiffObject = diffObject;
            try
            {
                //FText = new StringBuilder();
                curItem = curItem == null ? root : curItem.Add();
                curRoot = curItem;
                diffObject = diff;
                if (obj is Base && SerializeTo == SerializeTo.Preview)
                {
                    diffObject = (obj as Base).OriginalComponent;
                    curItem.Name = diffObject != null ? (obj as Base).Alias : (obj as Base).ClassName;
                }
                if (GetDiff != null)
                {
                    DiffEventArgs e = new DiffEventArgs();
                    e.Object = obj;
                    GetDiff(this, e);
                    diffObject = e.DiffObject;
                }
                if (diffObject == null)
                {
                    try
                    {
                        Type objType = obj.GetType();
                        if (!diffObjects.Contains(objType))
                            diffObjects[objType] = Activator.CreateInstance(objType);
                        diffObject = diffObjects[objType];
                    }
                    catch
                    {
                    }
                }
                obj.Serialize(this);
            }
            finally
            {
                //if (FText.Length > 0)
                //          FText.Remove(FText.Length - 1, 1);
                //FCurRoot.Text = FText.ToString();
                //FText = saveText;
                curItem = saveCurItem;
                curRoot = saveCurRoot;
                diffObject = saveDiffObject;
            }
        }

        /// <summary>
        /// Writes a string property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteStr(string name, string value)
        {
            curRoot.SetProp(PropName(name), value);
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(Converter.ToXml(value));
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes a boolean property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteBool(string name, bool value)
        {
            curRoot.SetProp(PropName(name), value ? "true" : "false");
            //      FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value ? "true" : "false");
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes an integer property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteInt(string name, int value)
        {
            curRoot.SetProp(PropName(name), value.ToString());
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value.ToString());
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes a float property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteFloat(string name, float value)
        {
            curRoot.SetProp(PropName(name), value.ToString(CultureInfo.InvariantCulture.NumberFormat));
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value.ToString(CultureInfo.InvariantCulture.NumberFormat));
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes a double property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteDouble(string name, double value)
        {
            curRoot.SetProp(PropName(name), value.ToString(CultureInfo.InvariantCulture.NumberFormat));
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value.ToString(CultureInfo.InvariantCulture.NumberFormat));
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes an enumeration property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteValue(string name, object value)
        {
            curRoot.SetProp(PropName(name), value != null ? Converter.ToString(value) : "null");
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value != null ? Converter.ToXml(value) : "null");
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes an object reference property.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public void WriteRef(string name, Base value)
        {
            curRoot.SetProp(PropName(name), value != null ? value.Name : "null");
            //FText.Append(PropName(name));
            //FText.Append("=\"");
            //FText.Append(value != null ? value.Name : "null");
            //FText.Append("\" ");
        }

        /// <summary>
        /// Writes a standalone property value.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="value">Property value.</param>
        /// <remarks>
        /// This method produces the following output:
        /// &lt;PropertyName&gt;PropertyValue&lt;/PropertyName&gt;
        /// </remarks>
        public void WritePropertyValue(string name, string value)
        {
            XmlItem item = curItem.Add();
            item.Name = name;
            item.Value = value;
        }

        /// <summary>
        /// Determines if two objects are equal.
        /// </summary>
        /// <param name="obj1">The first object.</param>
        /// <param name="obj2">The second object.</param>
        /// <returns><b>true</b> if objects will be serialized to the same value.</returns>
        public bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == obj2)
                return true;
            if (obj1 == null || obj2 == null)
                return false;
            string s1 = Converter.ToString(obj1);
            string s2 = Converter.ToString(obj2);
            return s1 == s2;
        }

        /// <summary>
        /// Disposes the writer.
        /// </summary>
        public void Dispose()
        {
            doc.Dispose();
            foreach (object obj in diffObjects.Values)
            {
                if (obj is IDisposable)
                    (obj as IDisposable).Dispose();
            }
        }

        /// <summary>
        /// Saves the writer output to a stream.
        /// </summary>
        /// <param name="stream">Stream to save to.</param>
        public void Save(Stream stream)
        {
            doc.AutoIndent = serializeTo == SerializeTo.Report;
            doc.WriteHeader = WriteHeader;
            doc.Save(stream);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>FRWriter</b> class with default settings.
        /// </summary>
        public FRWriter()
        {
            doc = new XmlDocument();
            root = doc.Root;
            //FText = new StringBuilder();
            saveChildren = true;
            writeHeader = true;
            diffObjects = new Hashtable();
        }

        /// <summary>
        /// Initializes a new instance of the <b>FRWriter</b> class with specified xml item that will 
        /// receive writer's output.
        /// </summary>
        /// <param name="root">The xml item that will receive writer's output.</param>
        public FRWriter(XmlItem root) : this()
        {
            this.root = root;
        }
    }
}


