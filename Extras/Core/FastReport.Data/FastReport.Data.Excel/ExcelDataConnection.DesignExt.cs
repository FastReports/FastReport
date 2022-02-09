using FastReport.Data.ConnectionEditors;
using System;

namespace FastReport.Data
{
    public partial class ExcelDataConnection
    {
        /// <inheritdoc/>
        public override void TestConnection()
        {
            InitConnection();
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "Excel: " + ConnectionString;
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new ExcelConnectionEditor();
        }
    }
}
