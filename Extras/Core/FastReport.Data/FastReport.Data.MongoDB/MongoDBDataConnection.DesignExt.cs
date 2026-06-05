using System;
using System.Collections.Generic;
using System.Data.Common;
using MongoDB.Driver;
using FastReport.Data.ConnectionEditors;
using System.Data;
using MongoDB.Bson;
using System.Threading;

namespace FastReport.Data
{
    public partial class MongoDBDataConnection : DataConnectionBase
    {
       
        public override Type GetConnectionType()
        {
            return typeof(MongoClient);
        }   

        public override string GetConnectionId()
        {
            MongoUrlBuilder builder = new MongoUrlBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.DatabaseName;
            }
            catch
            {
            }
            return "MongoDB: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new MongoDBConnectionEditor();
        }
		
		/// <inheritdoc/>
        public override void TestConnection()
        {           
            MongoClient client = new MongoClient(ConnectionString);
#if NET462
            var databases = client.ListDatabases();//.GetDatabase(dbName);
            var doc = databases.First();
#else
            var databases = client.ListDatabases(CancellationToken.None);
            var doc = databases.First();
#endif
        }
    }
}
