using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    public partial class IgniteDataConnection : DataConnectionBase
    {
        private IIgniteClient _client;

        // Dictionary to store the mapping between simple and full table names
        private Dictionary<string, string> _tableMapping = new Dictionary<string, string>();

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            // Initialize the connection if the client is not already initialized
            if (_client == null)
                InitConnection();

            // Retrieve the list of all cache names (tables) from Ignite
            var cacheNames = _client.GetCacheNames().ToList();

            // Clear the dictionary to avoid stale or duplicate mappings
            _tableMapping.Clear();

            // Convert full names to simple names and save the mapping between them
            return cacheNames.Select(fullName =>
            {
                // Generate a simplified version of the table name
                var simpleName = SimplifyTableName(fullName);
                // Save the mapping between the simple and full name in the dictionary
                _tableMapping[simpleName] = fullName;
                return simpleName;
            }).ToArray();
        }

        public override Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(GetTableNames());
        }

        /// <summary>
        /// Simplifies the full table name by removing the "SQL_" prefix and extracting the table name.<br/>
        /// If the "SQL_" prefix is absent, the original name is returned unchanged.
        /// </summary>
        /// <param name="fullName">Full table name.</param>
        /// <returns>Simplified table name.</returns>
        private static string SimplifyTableName(string fullName)
        {
            if (fullName.StartsWith("SQL_", StringComparison.OrdinalIgnoreCase))
            {
                // Removal of the "SQL_" prefix
                var remainingPart = fullName.Substring(4);

                // Splitting the remaining part into SCHEMA_NAME and TABLE_NAME
                var parts = remainingPart.Split('_');
                if (parts.Length >= 2)
                {
                    // Table name (considering that there may be multiple underscores)
                    var tableName = string.Join("_", parts.Skip(1));

                    // Return of the simplified table name
                    return tableName;
                }
            }

            // Return of the original name in the absence of the "SQL_" prefix
            return fullName;
        }

        /// <summary>
        /// Converts a simple table name back to its full form using the mapping dictionary. <br/>
        /// If the simple name is not found in the dictionary, it is assumed to already be the full name.
        /// </summary>
        /// <param name="simpleName">Simple table name.</param>
        /// <returns>Full table name.</returns>
        private string GetFullTableName(string simpleName)
        {
            if (_tableMapping.TryGetValue(simpleName.ToUpper(), out var fullName))
            {
                return fullName;
            }

            // If the simple name is not found, return it as is (assumed to already be the full name)
            return simpleName;
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        public override void CreateAllTables(bool initSchema)
        {
            if (_client == null)
                InitConnection();

            base.CreateAllTables(initSchema);
        }

        public override Task CreateAllTablesAsync(bool initSchema, CancellationToken cancellationToken = default)
        {
            CreateAllTables(initSchema);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (_client == null)
                InitConnection();

            base.CreateTable(source);
        }

        public override Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            CreateTable(source);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a DataTable based on data from the Apache Ignite cache.
        /// </summary>
        /// <param name="table">Target table to populate.</param>
        /// <param name="cache">Ignite cache containing the data.</param>
        /// <returns>Populated DataTable.</returns>
        public static DataTable CreateDataTable(DataTable table, ICacheClient<object, object> cache)
        {
            // Internal method to process the cursor and populate the DataTable
            void ProcessCursor(IQueryCursor<ICacheEntry<object, object>> cursor)
            {
                foreach (var entry in cursor)
                {
                    DataRow dr = table.NewRow();
                    // Populate the row with data from the cache entry
                    ExecuteFillDataTable(entry.Value, table, dr, string.Empty);
                    table.Rows.Add(dr);
                }
            }

            try
            {
                // Execute a query to retrieve all records from the cache
                var cursor = cache.Query(new ScanQuery<object, object>());

                ProcessCursor(cursor);
            }
            catch
            {
                // If an exception occurs while working with cache objects,
                // retry using KeepBinary mode

                // Possible causes of exceptions:
                // 1. BinaryObjectException: "No matching type found for object" – the cache contains custom classes that cannot be deserialized in the current context
                // 2. IgniteClientException: "Failed to resolve Java class" – the cache is created in Java and contains custom classes

                // Using WithKeepBinary allows working with data in binary format without deserialization

                var cursor = cache.WithKeepBinary<object, object>().Query(new ScanQuery<object, object>());

                ProcessCursor(cursor);
            }

            return table;
        }

        /// <summary>
        /// Recursively populates a DataTable with data from Apache Ignite.
        /// </summary>
        /// <param name="value">Value from the cache.</param>
        /// <param name="dt">Target table to be populated with data.</param>
        /// <param name="dr">Current row being populated during method execution.</param>
        /// <param name="parent">Parent prefix for nested fields.<br/>
        /// Used to create unique column names when processing nested structures.</param>
        private static void ExecuteFillDataTable(object value, DataTable dt, DataRow dr, string parent)
        {
            // Initialize a dictionary to track existing columns in the table
            var columnExists = dt.Columns.Cast<DataColumn>()
                .ToDictionary(col => col.ColumnName, col => true, StringComparer.OrdinalIgnoreCase);

            // Local function to add a column and set its value
            // If the column does not exist, it is added to the table
            void AddColumnAndSetValue(string colName, object fieldValue)
            {
                if (!columnExists.ContainsKey(colName))
                {
                    dt.Columns.Add(colName);
                    columnExists[colName] = true;
                }
                dr[colName] = fieldValue;
            }

            if (value is IDictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    string colName = string.IsNullOrEmpty(parent) ? kvp.Key : $"{parent}.{kvp.Key}";

                    AddColumnAndSetValue(colName, kvp.Value);
                }
            }
            else if (value is IBinaryObject binaryObject)
            {
                var binaryType = binaryObject.GetBinaryType();

                // If the binary object is an enumeration, store it as a string
                if (binaryType.IsEnum)
                {
                    AddColumnAndSetValue(parent, binaryObject.EnumName);
                }

                foreach (var fieldName in binaryType.Fields)
                {
                    // Get the value of a field by name
                    var fieldValue = binaryObject.GetField<object>(fieldName);
                    string colName = string.IsNullOrEmpty(parent) ? fieldName : $"{parent}.{fieldName}";

                    // If the field value is a nested binary object, process it recursively
                    if (fieldValue is IBinaryObject nestedBinaryObject)
                    {
                        ExecuteFillDataTable(nestedBinaryObject, dt, dr, colName);
                    }
                    else
                    {
                        AddColumnAndSetValue(colName, fieldValue);
                    }
                }
            }
            else
            {
                // Generate a unique name based on the number of columns in the table.
                string colName = string.IsNullOrEmpty(parent)
                                ? $"Value_{dt.Columns.Count}" // Root level
                                : $"{parent}.Value_{dt.Columns.Count}"; // Nested levels

                AddColumnAndSetValue(colName, value);
            }
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // If the client is not initialized, establish a connection
            if (_client == null)
                InitConnection();

            // If the _tableMapping dictionary is empty, retrieve the list of table names from the cache
            if (_tableMapping.Count == 0)
            {
                GetTableNames();
            }

            // Determine the full table name
            var fullTableName = GetFullTableName(table.TableName);

            var cache = _client.GetCache<object, object>(fullTableName);
            var queryEntities = cache.GetConfiguration().QueryEntities;

            if (queryEntities == null)
            {
                // If entity metadata is absent, create the DataTable based on data from the cache
                CreateDataTable(table, cache);
            }
            else
            {
                // If entity metadata exists, add columns to the table based on the metadata
                foreach (var entity in queryEntities)
                {
                    foreach (var item in entity.Fields)
                    {
                        // Skip fields with no data type, which is typical for custom classes
                        if (item.FieldType == null)
                            continue;
                        if (queryEntities.Count > 1)
                        {
                            table.Columns.Add(entity.TableName + "." + item.Name, item.FieldType);
                        }
                        else
                        {
                            table.Columns.Add(item.Name, item.FieldType);
                        }
                    }
                }
            }
        }

        public override Task FillTableSchemaAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            FillTableSchema(table, selectCommand, parameters);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // If the client is not initialized, establish a connection
            if (_client == null)
                InitConnection();

            // Convert the table name to its full name
            var fullTableName = GetFullTableName(table.TableName);

            var cache = _client.GetCache<object, object>(fullTableName);

            var queryEntities = cache?.GetConfiguration()?.QueryEntities;

            foreach (var entity in queryEntities ?? Enumerable.Empty<QueryEntity>())
            {
                var query = new SqlFieldsQuery($"SELECT * FROM {entity.TableName}");

                try
                {
                    // This attempts to execute the query without using the WithKeepBinary mode.
                    var cursor = cache.Query(query);
                    ProcessCursor(cursor, entity, queryEntities.Count > 1);
                }
                catch (BinaryObjectException)
                {
                    // If the cache contains custom classes, a deserialization error will occur.
                    // In this case, retry the query using the WithKeepBinary mode.
                    try
                    {
                        var cacheWithKeepBinary = _client.GetCache<object, object>(fullTableName).WithKeepBinary<object, object>();
                        var cursor = cacheWithKeepBinary.Query(query);
                        ProcessCursor(cursor, entity, queryEntities.Count > 1);
                    }
                    catch
                    {
                        // If both attempts fail, fall back to using the CreateDataTable method.
                        CreateDataTable(table, cache);
                    }
                }
            }

            // Processes the cursor and populates the table with data
            void ProcessCursor(IFieldsQueryCursor cursor, QueryEntity entity, bool isMultipleEntities)
            {
                if (isMultipleEntities)
                {
                    // Create a mapping between field names in the query and column names in the DataTable
                    var fieldMapping = new Dictionary<string, string>();
                    foreach (var field in entity.Fields)
                    {
                        var fieldNameInTable = $"{entity.TableName}.{field.Name}";
                        if (table.Columns.Contains(fieldNameInTable))
                        {
                            fieldMapping[field.Name.ToUpper()] = fieldNameInTable;
                        }
                    }

                    // Populate the table with data
                    foreach (var row in cursor)
                    {
                        var dataRow = table.NewRow();

                        for (int i = 0; i < row.Count; i++)
                        {
                            // Retrieve the field name from the query result
                            var fieldName = cursor.FieldNames[i];
                            if (fieldMapping.ContainsKey(fieldName))
                            {
                                // Get the corresponding column name from the mapping
                                var columnName = fieldMapping[fieldName];
                                dataRow[columnName] = row[i];
                            }
                        }

                        table.Rows.Add(dataRow);
                    }
                }
                else
                {
                    foreach (var row in cursor)
                    {
                        var dataRow = table.NewRow();
                        // When the cache contains custom data types,
                        // the objects are not directly serialized into table columns.
                        // Instead, only the fields of these objects are mapped to columns.
                        // This may result in a dimension mismatch:
                        // System.ArgumentException: "The input array length exceeds the number of table columns."
                        // In such cases, the CreateDataTable method is executed to create the table based on data from the cache.
                        dataRow.ItemArray = row.ToArray();
                        table.Rows.Add(dataRow);
                    }
                }
            }
        }

        public override Task FillTableDataAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            FillTableData(table, selectCommand, parameters);
            return Task.CompletedTask;
        }

        public IgniteDataConnection()
        {
            IsSqlBased = false;
        }

        /// <summary>
        /// Initializes a connection to Apache Ignite.
        /// </summary>
        /// <returns> An instance of the Apache Ignite client (IIgniteClient).</returns>
        public IIgniteClient InitConnection()
        {
            var clientConfig = ParseConnectionString(ConnectionString);

            try
            {
                // Initialize the Apache Ignite client
                _client = Ignition.StartClient(clientConfig);

                // Check if authentication is enabled
                bool isAuthenticationEnabled = !string.IsNullOrEmpty(clientConfig.UserName) &&
                                               !string.IsNullOrEmpty(clientConfig.Password);

                if (isAuthenticationEnabled)
                {
                    // Checking the cluster's activity is necessary when authentication is used,
                    // as authentication is typically combined with Persistent Storage
                    // When using Persistent Storage, the cluster starts in an inactive state,
                    // and it must be explicitly activated to perform data operations
                    // Without activation, accessing data is not possible

                    // Retrieve the cluster state
                    var clusterState = _client.GetCluster();

                    // If the cluster is inactive, activate it
                    if (!clusterState.IsActive())
                    {
                        clusterState.SetActive(true);
                    }
                }
            }
            catch (IgniteClientException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            return _client;
        }

        /// <summary>
        /// Parses the connection string and returns the Apache Ignite client configuration.
        /// </summary>
        /// <param name="connectionString">Connection string for Apache Ignite.</param>
        /// <returns>Apache Ignite client configuration.</returns>
        private static IgniteClientConfiguration ParseConnectionString(string connectionString)
        {
            var config = new IgniteClientConfiguration();

            // If the connection string is empty, return the default configuration
            if (string.IsNullOrWhiteSpace(connectionString))
                return config;

            var parts = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                // Split each part into key and value using the '=' delimiter
                var keyValue = part.Split(new[] { '=' }, 2);
                // Skip invalid key-value pairs
                if (keyValue.Length != 2 || string.IsNullOrWhiteSpace(keyValue[0]) || string.IsNullOrWhiteSpace(keyValue[1]))
                {
                    continue;
                }

                var key = keyValue[0].Trim().ToLowerInvariant();
                var value = keyValue[1].Trim();

                switch (key)
                {
                    case "endpoints":
                        // Split the list of endpoints by commas and trim whitespace
                        config.Endpoints = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(p => p.Trim())
                                                .Where(p => !string.IsNullOrWhiteSpace(p))
                                                .ToArray();
                        break;
                    case "username":
                        config.UserName = value;
                        break;
                    case "password":
                        config.Password = value;
                        break;
                }
            }
            return config;
        }
    }
}
