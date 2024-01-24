using System.IO;

namespace FastReport.Import
{
    /// <summary>
    /// Base class for all import plugins.
    /// </summary>
    public class ImportBase
    {
        #region Fields

        private string name;
        private Report report;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the name of plugin.
        /// </summary>
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// Gets or sets reference to the report.
        /// </summary>
        public Report Report
        {
            get { return report; }
            protected set { report = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportBase"/> class with default settings.
        /// </summary>
        public ImportBase()
        {
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Loads the specified file into specified report.
        /// </summary>
        /// <param name="report">Report object.</param>
        /// <param name="filename">File name.</param>
        public virtual void LoadReport(Report report, string filename)
        {
            report.Clear();
        }

        /// <summary>
        /// Loads the specified file into specified report from stream.
        /// </summary>
        /// <param name="report">Report object</param>
        /// <param name="content">File stream</param>
        public virtual void LoadReport(Report report, Stream content)
        {
            report.Clear();
        }

        #endregion // Public Methods
    }
}
