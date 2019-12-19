using FastReport.Data.JsonConnection.JsonParser;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Data.JsonConnection
{
    /// <summary>
    /// JsonTableDataSource present a json array object
    /// </summary>
    public class JsonTableDataSource : TableDataSource
    {
        #region Private Fields

        private JsonArray _json;
        private bool updateSchema;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets value for force update schema on init schema
        /// </summary>
        public bool UpdateSchema
        {
            get
            {
                return updateSchema;
            }
            set
            {
                updateSchema = value;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal JsonArray Json
        {
            get
            {
                if (_json == null)
                {
                    _json = GetJson(Parent, this) as JsonArray;
                    if (_json == null)
                        _json = new JsonArray();
                }
                return _json;
            }
            set
            {
                _json = value;
            }
        }

        #endregion Internal Properties

        #region Private Properties

        private int CurrentIndex
        {
            get
            {
                if (currentRow is int)
                    return (int)currentRow;
                if (CurrentRowNo < Rows.Count)
                {
                    object result = Rows[CurrentRowNo];
                    if (result is int)
                        return (int)result;
                }
                return CurrentRowNo;
            }
        }

        #endregion Private Properties

        #region Public Constructors

        /// <inheritdoc/>
        public JsonTableDataSource()
        {
            DataType = typeof(JsonArray);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            Json = null;
        }

        /// <inheritdoc/>
        public override void InitSchema()
        {
            if (Columns.Count == 0 || UpdateSchema)
            {
                if (Connection is JsonDataSourceConnection)
                {
                    JsonDataSourceConnection con = Connection as JsonDataSourceConnection;

                    InitSchema(this, con.JsonSchema);
                }
            }
            UpdateSchema = false;
        }

        /// <inheritdoc/>
        public override void LoadData(ArrayList rows)
        {
            Json = null;
            if ( rows != null)
            {
                rows.Clear();
                int count = Json.Count;
                for (int i = 0; i < count; i++)
                {
                    rows.Add(i);
                }
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal static object GetJsonBaseByColumn(Base parentColumn, Column column)
        {
            if (parentColumn is JsonTableDataSource)
            {
                switch (column.PropName)
                {
                    case "item":
                        return (parentColumn as JsonTableDataSource).Json[(parentColumn as JsonTableDataSource).CurrentIndex];
                }
                JsonTableDataSource source = column as JsonTableDataSource;
                return source.Json;
            }
            if (parentColumn is Column && !String.IsNullOrEmpty(column.PropName))
            {
                object json = GetJsonBaseByColumn(parentColumn.Parent, parentColumn as Column);
                if (json is JsonBase)
                {
                    return (json as JsonBase)[column.PropName];
                }
            }

            return null;
        }

        internal static object GetValueByColumn(Base parentColumn, Column column)
        {
            if (parentColumn is JsonTableDataSource)
            {
                switch (column.PropName)
                {
                    case "index":
                        return (parentColumn as JsonTableDataSource).CurrentIndex;

                    case "array":
                        return (parentColumn as JsonTableDataSource).Json;
                }
            }

            object json = GetJsonBaseByColumn(parentColumn, column);

            return json;
        }

        internal static void InitSchema(Column table, JsonSchema schema)
        {
            List<Column> saveColumns = new List<Column>();
            switch (schema.Type)
            {
                case "object":
                    foreach (KeyValuePair<string, JsonSchema> kv in schema.Properties)
                    {
                        if (kv.Value.Type == "object")
                        {
                            Column c = new Column();
                            c.Name = kv.Key;
                            c.Alias = kv.Key;
                            c.PropName = kv.Key;
                            c.DataType = kv.Value.DataType;
                            c = UpdateColumn(table, c, saveColumns);
                            InitSchema(c, kv.Value);
                        }
                        else if (kv.Value.Type == "array")
                        {
                            Column c = new JsonTableDataSource();
                            c.Name = kv.Key;
                            c.Alias = kv.Key;
                            c.PropName = kv.Key;
                            c.DataType = kv.Value.DataType;
                            c = UpdateColumn(table, c, saveColumns);
                            InitSchema(c, kv.Value);
                        }
                        else
                        {
                            Column c = new Column();
                            c.Name = kv.Key;
                            c.Alias = kv.Key;
                            c.PropName = kv.Key;
                            c.DataType = kv.Value.DataType;
                            c.SetBindableControlType(c.DataType);
                            UpdateColumn(table, c, saveColumns);
                        }
                    }
                    break;

                case "array":
                    JsonSchema items = schema.Items;
                    {
                        Column c = new Column();
                        c.Name = "index";
                        c.Alias = "index";
                        c.PropName = "index";
                        c.DataType = typeof(int);
                        UpdateColumn(table, c, saveColumns);
                    }

                    {
                        Column c;
                        bool iSchema = false;

                        if (items.Type == "object")
                        {
                            c = new Column();
                            iSchema = true;

                        }
                        else if (items.Type == "array")
                        {
                            c = new JsonTableDataSource();
                            iSchema = true;
                        }
                        else
                        {
                            c = new Column();
                        }

                        c.Name = "item";
                        c.Alias = "item";
                        c.PropName = "item";
                        c.DataType = items.DataType;
                        c = UpdateColumn(table, c, saveColumns);

                        if (iSchema)
                            InitSchema(c, items);
                    }

                    {
                        Column c = new Column();
                        c.Name = "array";
                        c.Alias = "array";
                        c.PropName = "array";
                        c.DataType = typeof(JsonBase);
                        UpdateColumn(table, c, saveColumns);
                    }
                    break;
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (!(table.Columns[i].Calculated || saveColumns.Contains(table.Columns[i])))
                {
                    table.Columns.RemoveAt(i);
                    i--;
                }
            }
        }

        internal object GetJson(Base parentColumn, Column column)
        {
            if (parentColumn is JsonDataSourceConnection)
            {
                // return zero level json
                return (parentColumn as JsonDataSourceConnection).Json;
            }
            else if (parentColumn is JsonTableDataSource)
            {
                JsonTableDataSource source = parentColumn as JsonTableDataSource;
                return source.Json[source.CurrentRowNo] as object;
            }
            else if (parentColumn is Column)
            {
                object parentJson = GetJson(parentColumn.Parent, parentColumn as Column);
                if (parentJson is JsonBase && !String.IsNullOrEmpty(column.PropName))
                {
                    return (parentJson as JsonBase)[column.PropName];
                }
            }
            return null;
        }

        #endregion Internal Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override object GetValue(Column column)
        {
            return GetValueByColumn(column.Parent, column);
        }

        /// <inheritdoc/>
        protected override object GetValue(string alias)
        {
            // TODO TEST
            Column column = this;
            string[] colAliases = alias.Split(new char[] { '.' });

            foreach (string colAlias in colAliases)
            {
                column = column.Columns.FindByAlias(colAlias);
                if (column == null)
                    return null;
            }

            return GetValueByColumn(column.Parent, column);
        }

        #endregion Protected Methods

        #region Private Methods

        private static Column UpdateColumn(Column table, Column c, List<Column> list)
        {
            foreach (Column child in table.Columns)
            {
                if (child.PropName == c.PropName)
                {
                    child.DataType = c.DataType;
                    list.Add(child);
                    return child;
                }
            }
            table.AddChild(c);
            list.Add(c);
            return c;
        }

        #endregion Private Methods
    }
}