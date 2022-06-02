using FastReport.Data.ConnectionEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FastReport.Data
{
    public partial class LinterDataConnection
    {
        #region Public Methods

        /// <inheritdoc/>
        public override void TestConnection()
        {
            Init();
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new LinterDataConnectionEditor(factory);
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "LINTER: " + ConnectionString;
        }

        #endregion Public Methods
    }
}
