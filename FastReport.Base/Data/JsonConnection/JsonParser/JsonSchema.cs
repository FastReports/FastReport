using System;
using System.Collections.Generic;

namespace FastReport.Data.JsonConnection.JsonParser
{
    internal class JsonSchema
    {
        #region Private Fields

        private string description;
        private JsonSchema items;
        private Dictionary<string, JsonSchema> properties;
        private string type;

        #endregion Private Fields

        #region Public Properties

        public Type DataType
        {
            get
            {
                switch (type)
                {
                    case "object": return typeof(JsonBase);
                    case "array": return typeof(JsonBase);
                    case "integer": return typeof(int);
                    case "number": return typeof(double);
                    case "string": return typeof(string);
                    case "boolean": return typeof(bool);
                    default: return typeof(object);
                }
            }
        }

        public string Description { get { return description; } set { description = value; } }

        public JsonSchema Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        public Dictionary<string, JsonSchema> Properties
        {
            get
            {
                return properties;
            }
        }

        public string Type { get { return type; } set { type = value; } }

        #endregion Public Properties

        #region Public Methods

        public static JsonSchema FromJson(object json)
        {
            JsonSchema result = new JsonSchema();
            if (json is JsonObject)
            {
                result.type = "object";
                result.properties = new Dictionary<string, JsonSchema>();

                foreach (KeyValuePair<string, object> kv in (json as JsonObject))
                {
                    result.Properties[kv.Key] = FromJson(kv.Value);
                }
            }
            else if (json is JsonArray)
            {
                result.Type = "array";
                result.items = null;
                foreach (object obj in (json as JsonArray))
                {
                    JsonSchema sub = FromJson(obj);
                    if (result.items == null)
                        result.items = sub;
                    else result.items.Union(sub);
                }

                if (result.items == null)
                    result.items = new JsonSchema();
            }
            else if (json is string)
            {
                result.Type = "string";
            }
            else if (json is double)
            {
                result.Type = "number";
            }
            else if (json is bool)
            {
                result.Type = "boolean";
            }
            else
            {
                result.Type = "null";
            }
            return result;
        }

        public static JsonSchema Load(JsonObject obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("Unable to load schema from non-object");
            }

            JsonSchema result = new JsonSchema();
            if (obj != null)
            {
                result.Type = obj.ReadString("type");
                result.Description = obj.ReadString("description");
                switch (result.Type)
                {
                    case "object":
                        result.properties = new Dictionary<string, JsonSchema>();
                        if (obj.ContainsKey("properties"))
                        {
                            JsonObject child = obj["properties"] as JsonObject;
                            if (child != null)
                            {
                                foreach (KeyValuePair<string, object> kv in child)
                                {
                                    if (kv.Value is JsonObject)
                                    {
                                        result.Properties[kv.Key] = Load(kv.Value as JsonObject);
                                    }
                                    else
                                    {
                                        result.Properties[kv.Key] = new JsonSchema();
                                    }
                                }
                            }
                        }
                        break;

                    case "array":
                        if (obj.ContainsKey("items"))
                            result.items = Load(obj["items"] as JsonObject);
                        else
                            result.items = new JsonSchema();
                        break;
                }
            }

            return result;
        }

        public void Save(JsonObject obj)
        {
            if (Type != null)
            {
                obj["type"] = Type;
            }
            if (Description != null)
            {
                obj["description"] = Description;
            }

            if (Items != null)
            {
                JsonObject child = new JsonObject();
                Items.Save(child);
                obj["items"] = child;
            }

            if (Properties != null && Properties.Count > 0)
            {
                JsonObject child = new JsonObject();
                obj["properties"] = child;
                foreach (KeyValuePair<string, JsonSchema> kv in Properties)
                {
                    JsonObject sub_child = new JsonObject();
                    kv.Value.Save(sub_child);
                    child[kv.Key] = sub_child;
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Union(JsonSchema sub)
        {
            if (sub == null || type != sub.type)
            {
                items = null;
                properties = null;
                type = "null";
            }
            else if (type == "object")
            {
                if (properties == null)
                    properties = new Dictionary<string, JsonSchema>();
                if (sub.Properties != null)
                    foreach (KeyValuePair<string, JsonSchema> kv in sub.Properties)
                    {
                        JsonSchema child;
                        if (Properties.TryGetValue(kv.Key, out child))
                        {
                            child.Union(kv.Value);
                        }
                        else
                        {
                            Properties[kv.Key] = kv.Value;
                        }
                    }
            }
            else if (type == "array")
            {
                if (items == null)
                    items = sub.items;
                else
                {
                    items.Union(sub.items);
                }
            }
        }

        #endregion Private Methods
    }
}