using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using FastReport.Data.ConnectionEditors;
using Raven.Client;
using Raven.Client.Documents;
using System.Net;
using System.ComponentModel;
//using Raven.Json.Linq;
//using Raven.Imports.Newtonsoft.Json.Linq;
using Raven.Client.Documents.Session;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to RavenDB database.
    /// </summary>
    public partial class RavenDBDataConnection : DataConnectionBase
    {
                /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(RavenDBDataConnection);
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
            return "RavenDB: " + builder.ToString();
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new RavenDBConnectionEditor();
        }

        /// <inheritdoc/>
        public override void TestConnection()
        {
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

            store.Initialize();
            IDocumentSession session = store.OpenSession();
            store.Dispose();

            //using (var store = new DocumentStore())
            //{
            //    store.Url = Host;
            //    if (!string.IsNullOrEmpty(DatabaseName))
            //        store.DefaultDatabase = DatabaseName;
            //    if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
            //        store.Credentials = new NetworkCredential(Username, Password);
            //    store.Initialize();
            //    IDocumentSession session = store.OpenSession();
            //}
        }
    }
}
