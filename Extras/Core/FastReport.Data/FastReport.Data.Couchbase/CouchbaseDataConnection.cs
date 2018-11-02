using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ComponentModel;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Authentication;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to Couchbase database.
    /// </summary>
    public partial class CouchbaseDataConnection : JsonDataConnection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [Category("Data")]
        public string DatabaseName
        {
            get
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                return builder.DatabaseName;
            }
            set
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
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
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                return builder.Host;
            }
            set
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                builder.Host = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Category("Data")]
        public string Username
        {
            get
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                return builder.Username;
            }
            set
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                builder.Username = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Category("Data")]
        public string Password
        {
            get
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                return builder.Password;
            }
            set
            {
                CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
                builder.Password = value;
                ConnectionString = builder.ToString();
            }
        }

        #endregion Properties

        #region Protected Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            using (var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(Host) }
            }))
            {
                var authenticator = new PasswordAuthenticator(Username, Password);
                cluster.Authenticate(authenticator);
                var bucket = cluster.OpenBucket(DatabaseName);
                List<string> entityNames = cluster.Query<JObject>("select distinct `type` from `beer-sample`")
                    .Rows.Select(v => v.GetValue("type").ToString()).ToList();

                StringBuilder json = new StringBuilder();
                json.Append("{\n");
                foreach (string name in entityNames)
                {
                    json.Append("\"").Append(name).Append("\":\n[");
                    DataTable table = new DataTable(name);

                    // get all rows of the table
                    var objects = cluster.Query<JObject>($"select * from `{DatabaseName}` where type = \"{name}\"")
                    .Rows.ToList();
                    foreach (var jobj in objects)
                    {
                        json.Append(jobj.GetValue(DatabaseName).ToString());
                        if (jobj != objects.Last())
                            json.Append(",");
                    }
                    json.Append("]\n");
                    if (name != entityNames.Last())
                        json.Append(",");
                }
                json.Append("}");
                var jsstring = json.ToString();
                JsonData = jsstring;
                return base.CreateDataSet();
            }
        }

        #endregion Protected Methods
    }
}
