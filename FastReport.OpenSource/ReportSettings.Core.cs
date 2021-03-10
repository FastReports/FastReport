using FastReport.Data;
using System;

namespace FastReport
{
    partial class ReportSettings
    {
        #region Internal Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="report"></param>
        internal void OnFinishProgress(Report report)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="report"></param>
        /// <param name="str"></param>
        internal void OnProgress(Report report, string str)
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="report"></param>
        internal void OnProgress(Report report, string str, int int1, int int2)
        {

        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="report"></param>
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