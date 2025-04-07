using FastReport.Data.ConnectionEditors;
using System;

namespace FastReport.Data
{
    public partial class IgniteDataConnection : DataConnectionBase
    {
        /// <inheritdoc/>
        public override void TestConnection()
        {
            InitConnection();
        }

        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(IgniteDataConnection);
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "Ignite: " + ConnectionString;
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new IgniteConnectionEditor();
        }
    }
}