using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Raven.Client;
using Raven.Client.Documents;
using System.Net;
using System.ComponentModel;
using Raven.Client.Documents.Session;
using Sparrow.Json;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents.Operations.Indexes;
using Raven.Client.Documents.Operations;
using System.Security.Cryptography.X509Certificates;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to RavenDB database.
    /// </summary>
    public partial class RavenDBDataConnection : DataConnectionBase
    {
        private X509Certificate2 certificate;

        //public string CertificatePath { get; set; } //= @"C:\Users\alexe\Downloads\RavenDB-4.0.2-windows-x64\raven1337.Cluster.Settings\admin.client.certificate.raven1337.pfx";


        #region Properties
        public X509Certificate2 Certificate
        {
            get
            {
                if (!string.IsNullOrEmpty(CertificatePath))
                {
                    if (!string.IsNullOrEmpty(this.Password))
                        certificate = new X509Certificate2(CertificatePath, Password);
                    else
                        certificate = new X509Certificate2(CertificatePath);
                }
                return certificate;
            }
            set
            {
                certificate = value;
            }
        }

        /// <summary>
        /// Gets or sets the certificate path.
        /// </summary>
        [Category("Data")]
        public string CertificatePath
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.CertificatePath;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.CertificatePath = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the certificate password.
        /// </summary>
        [Category("Data")]
        [PasswordPropertyText(true)]
        public string Password
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.Password;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.Password = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [Category("Data")]
        public string DatabaseName
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.DatabaseName;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.DatabaseName = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the host url.
        /// </summary>
        [Category("Data")]
        public string Host
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.Host;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.Host = value;
                ConnectionString = builder.ToString();
            }
        }

        #endregion Properties

        #region Protected Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            DataSet dataset = base.CreateDataSet();

            IDocumentStore store = this.Certificate == null ? new DocumentStore()
            {
                Urls = new string[] { Host },
                Database = DatabaseName
            }
            : new DocumentStore()
            {
                Urls = new string[] { Host },
                Database = DatabaseName,
                Certificate = this.Certificate
            };

            //using strt  {
            store.Initialize();
            using (IDocumentSession session = store.OpenSession())
            {
                var operation = new GetCollectionStatisticsOperation();
                var ress = store.Maintenance.Send(operation);
                List<string> entityNames = ress.Collections.Select(c => c.Key).ToList();
                foreach (string name in entityNames)
                {
                    DataTable table = new DataTable(name);

                    #region Local Functions
                    void AddColumn(string colName, Type colType)
                    {
                        try
                        {

                            DataColumn column = new DataColumn();
                            column.ColumnName = colName;
                            column.DataType = colType ?? typeof(string);
                            table.Columns.Add(column);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    Type GetSimpleType(Type columnType)
                    {
                        try
                        {
                            if (columnType == typeof(LazyStringValue))
                                columnType = typeof(string);
                            else if (columnType == typeof(LazyNumberValue))
                            {

                            }
                            return columnType;
                        }
                        catch (Exception ex)
                        {

                        }
                        return typeof(string);
                    }
                    #endregion

                    // get all rows of the table
                    BlittableJsonReaderObject[] objects = session.Advanced.LoadStartingWith<BlittableJsonReaderObject>(name, null, 0, Int32.MaxValue);
                    if (objects.Count() > 0)
                    {
                        //create table columns
                        var properties = objects[0].GetPropertyNames();
                        foreach (var prop in properties)
                        {
                            try
                            {
                                var item = objects[0][prop];
                                Type columnType = item?.GetType() ?? typeof(string);
                                //columns
                                if (columnType != typeof(BlittableJsonReaderObject))
                                {
                                    columnType = GetSimpleType(columnType);
                                    AddColumn(prop, columnType);
                                }
                                //subcolumns
                                else if (columnType == typeof(BlittableJsonReaderArray))
                                {

                                }
                                else
                                {
                                    var complexItem = (item as BlittableJsonReaderObject);
                                    var subproperties = complexItem.GetPropertyNames();
                                    foreach (var subprop in subproperties)
                                    {
                                        var subitem = complexItem[subprop];
                                        columnType = GetSimpleType(subprop.GetType() ?? typeof(string));
                                        AddColumn($"{prop}.{subprop}", columnType);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        // add table rows
                        foreach (var obj in objects)
                        {
                            DataRow row = table.NewRow();
                            // add row cells
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                if (table.Columns[i].ColumnName.Contains("."))
                                {
                                    string[] parts = table.Columns[i].ColumnName.Split(".".ToCharArray());
                                    var value = obj[parts[0]];

                                    if (value is BlittableJsonReaderObject)
                                    {
                                        var subvalue = (value as BlittableJsonReaderObject)[parts[1]];
                                        row[i] = subvalue;
                                    }
                                }
                                else
                                {
                                    var value = obj[table.Columns[i].ColumnName];
                                    row[i] = value;
                                }
                            }
                            table.Rows.Add(row);
                        }
                        dataset.Tables.Add(table);
                    }
                }

            }
            store.Dispose();
            return dataset;
            //using end }

        }

#endregion Protected Methods

#region Public Methods

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (DataSet.Tables.Contains(source.TableName))
            {
                source.Table = DataSet.Tables[source.TableName];
                base.CreateTable(source);
            }
            else
            {
                source.Table = null;
            }
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            string[] result = new string[DataSet.Tables.Count];
            for (int i = 0; i < DataSet.Tables.Count; i++)
            {
                result[i] = DataSet.Tables[i].TableName;
            }
            return result;
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

#endregion Public Methods
    }
}
