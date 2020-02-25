using System;
using System.Collections.Generic;

namespace FastReport.Utils
{
    /// <summary>
    /// Script security event arguments.
    /// </summary>
    public class ScriptSecurityEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the report language.
        /// </summary>
        /// <value>The report language.</value>
        public Language ReportLanguage
        {
            get
            {
                return Report.ScriptLanguage;
            }
        }

        private Report _report;

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <value>The report.</value>
        public Report Report
        {
            get
            {
                return _report;
            }
        }

        private string _reportScript;

        /// <summary>
        /// Gets the report script.
        /// </summary>
        /// <value>The report script.</value>
        public string ReportScript
        {
            get
            {
                return _reportScript;
            }
        }

        private string[] _references;

        /// <summary>
        /// Gets the references of script.
        /// </summary>
        /// <value>Script references</value>
        public string[] References
        {
            get
            {
                return _references;
            }
        }

        private bool _isValid = true;

        /// <summary>
        /// Gets or sets value if script is allowed to compile
        /// </summary>
        /// <value><c>true</c> if is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FastReport.Utils.ScriptSecurityEventArgs"/> class.
        /// </summary>
        /// <param name="report">Report.</param>
        /// <param name="script">Report's script.</param>
        /// <param name="refs">Report's references.</param>
        public ScriptSecurityEventArgs(Report report, string script, string[] refs)
        {
            _report = report;
            _reportScript = script;
            _references = refs;
        }
    }
}
