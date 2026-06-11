using FastReport.Data;
using System;

namespace FastReport
{
    partial class ReportSettings
    {
        #region Internal Methods

        // does nothing
        internal void OnFinishProgress(Report report)
        {
        }

        // does nothing
        internal void OnProgress(Report report, string str)
        {
        }

        // does nothing
        internal void OnProgress(Report report, string str, int int1, int int2)
        {

        }

        // does nothing
        internal void OnStartProgress(Report report)
        {
        }

        internal void OnDatabaseLogin(DataConnectionBase sender, DatabaseLoginEventArgs e)
        {
            if (DatabaseLogin != null)
                DatabaseLogin(sender, e);
        }

        #endregion Internal Methods
    }
}