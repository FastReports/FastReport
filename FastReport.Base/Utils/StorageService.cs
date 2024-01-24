using System;
using System.Globalization;

namespace FastReport.Utils
{
    /// <summary>
    /// Storage service that replaces direct manipulations with Config.Root xml storage.
    /// </summary>
    public class StorageService
    {
        private string path;
        private XmlItem root;

        internal XmlItem Root
        {
            get
            {
                if (root == null)
                    Init();
                return root;
            }
        }

        private void Init()
        {
            string[] pathElements = path.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            root = Config.Root;
            foreach (string element in pathElements)
            {
                root = root.FindItem(element);
            }
        }

        /// <summary>
        /// Determines if the key has a value.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key has a non-empty value.</returns>
        public bool Has(string key) => GetStr(key) != "";

        /// <summary>
        /// Determines if the storage is not empty.
        /// </summary>
        public bool HasProperties => !Root.IsNullOrEmptyProps();

        /// <summary>
        /// Gets a string value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>The value associated with a key, or default value.</returns>
        public string GetStr(string key, string defaultValue = "")
        {
            string value = Root.GetProp(key);
            if (value != "")
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets a bool value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>The value associated with a key, or default value.</returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            string value = GetStr(key);
            if (!string.IsNullOrEmpty(value))
                return value == "1" || value.ToLower() == "true";
            return defaultValue;
        }

        /// <summary>
        /// Gets an int value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>The value associated with a key, or default value.</returns>
        public int GetInt(string key, int defaultValue = 0)
        {
            if (int.TryParse(GetStr(key), out int val))
                return val;
            return defaultValue;
        }

        /// <summary>
        /// Gets a float value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>The value associated with a key, or default value.</returns>
        public float GetFloat(string key, float defaultValue = 0)
        {
            if (float.TryParse(GetStr(key).Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                return val;
            return defaultValue;
        }

        /// <summary>
        /// Gets an enum value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>The value associated with a key, or default value.</returns>
        public T GetEnum<T>(string key, T defaultValue = default(T)) where T : struct
        {
            string value = GetStr(key);
            if (Enum.TryParse<T>(value, out T result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Reads a serializable object.
        /// </summary>
        /// <param name="obj">The object to read.</param>
        public void Read(IFRSerializable obj)
        {
            using (FRReader reader = new FRReader(null, Root))
            {
                reader.Read(obj);
            }
        }

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetStr(string key, string value)
        {
            Root.SetProp(key, value);
        }

        /// <summary>
        /// Sets a bool value as a 0/1.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetBool(string key, bool value)
        {
            SetStr(key, value ? "1" : "0");
        }

        /// <summary>
        /// Sets a bool value as a False/True.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetBoolStr(string key, bool value)
        {
            SetStr(key, value ? "True" : "False");
        }

        /// <summary>
        /// Sets an int value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetInt(string key, int value)
        {
            SetStr(key, value.ToString());
        }

        /// <summary>
        /// Sets a float value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetFloat(string key, float value)
        {
            SetStr(key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Sets an enum value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Value associated with a key.</param>
        public void SetEnum<T>(string key, T value) where T : struct
        {
            SetStr(key, value.ToString());
        }

        /// <summary>
        /// Writes a serializable object.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        public void Write(IFRSerializable obj)
        {
            using (FRWriter writer = new FRWriter(Root))
            {
                Root.Items.Clear();
                writer.Write(obj);
            }
        }

        /// <summary>
        /// Initializes a new instance of a storage.
        /// </summary>
        /// <param name="commaSeparatedPath">The comma-separated path.</param>
        public StorageService(string commaSeparatedPath)
        {
            path = commaSeparatedPath;
        }

    }
}
