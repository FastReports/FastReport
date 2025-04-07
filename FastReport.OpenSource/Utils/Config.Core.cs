using FastReport.Data;

namespace FastReport.Utils
{
    static partial class Config
    {
        #region Public Properties

        /// <summary>
        /// Provides data for the FilterConnectionTables event.
        /// </summary>
        public class FilterConnectionTablesEventArgs
        {

            /// <summary>
            /// Gets the Connection object.
            /// </summary>
            public DataConnectionBase Connection { get; }

            /// <summary>
            /// Gets the table name.
            /// </summary>
            public string TableName { get; }

            /// <summary>
            /// Gets or sets a value that indicates whether this table should be skipped.
            /// </summary>
            public bool Skip { get; set; }

            internal FilterConnectionTablesEventArgs(DataConnectionBase connection, string tableName)
            {
                this.Connection = connection;
                this.TableName = tableName;
            }
        }

        /// <summary>
        /// Occurs when getting available table names from the connection.
        /// </summary>
        /// <remarks>
        /// Use this handler to filter the list of tables returned by the connection object.
        /// </remarks>
        /// <example>
        /// This example demonstrates how to hide the table with "Table 1" name from the Data Wizard.
        /// <code>
        /// Config.FilterConnectionTables += FilterConnectionTables;
        ///
        /// private void FilterConnectionTables(object sender, FilterConnectionTablesEventArgs e)
        /// {
        ///   if (e.TableName == "Table 1")
        ///     e.Skip = true;
        /// }
        /// </code>
        /// </example>
        public static event FilterConnectionTablesEventHandler FilterConnectionTables;

        /// <summary>
        /// Represents the method that will handle the FilterConnectionTables event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        public delegate void FilterConnectionTablesEventHandler(object sender, FilterConnectionTablesEventArgs e);

        internal static void OnFilterConnectionTables(object sender, FilterConnectionTablesEventArgs e)
        {
            if (FilterConnectionTables != null)
                FilterConnectionTables(sender, e);
        }

        /// <summary>
        /// Gets a value indicating that the ASP.NET hosting permission level is set to full trust.
        /// </summary>
        public static bool FullTrust
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value that determines whether to disable some functionality to run in web mode.
        /// </summary>
        /// <remarks>
        /// Use this property if you use FastReport in ASP.Net. Set this property to <b>true</b> <b>before</b>
        /// you access any FastReport.Net objects.
        /// </remarks>
        public static bool WebMode
        {
            get
            {
                return FWebMode;
            }
            set
            {
                FWebMode = value;
            }
        }
        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        static partial void RestoreUIStyle();

        /// <summary>
        /// Does nothing
        /// </summary>
        static partial void SaveUIStyle();

        static partial void SaveUIOptions();

        static partial void RestoreUIOptions();

        static partial void SaveExportOptions();

        static partial void SaveAuthServiceUser();

        static partial void RestoreAuthServiceUser();

        static partial void RestoreExportOptions();

        #endregion Private Methods

        internal static void DoEvent()
        {
            // do nothing
        }
    }
}