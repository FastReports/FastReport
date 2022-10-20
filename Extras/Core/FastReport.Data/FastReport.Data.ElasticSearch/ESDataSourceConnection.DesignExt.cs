using FastReport.Data.ConnectionEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Data.ElasticSearch
{
    partial class ESDataSourceConnection
    {
        #region Properties
        public ESConnectionEditor Editor { get; set; }
        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return Editor = new ESConnectionEditor();
        }
        /// <inheritdoc/>
        public override void TestConnection()
        {
            InitConnection();
        }

        public override string GetConnectionId()
        {
            ESDataSourceConnectionStringBuilder connectionStringBuilder = new ESDataSourceConnectionStringBuilder(ConnectionString);
            if (connectionStringBuilder.EndPoint.Length > 1)
                return "ElasticSearch: " + connectionStringBuilder.EndPoint;
            else return "ElasticSearch: ";
        }
        #endregion
    }
}
