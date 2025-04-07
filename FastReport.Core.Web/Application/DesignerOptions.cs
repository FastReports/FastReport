using System.Collections.Generic;
using System;

#nullable enable

namespace FastReport.Web
{
    public class DesignerOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether custom SQL queries can be executed.
        /// When set to true, users will be able to specify custom table names and SQL expressions
        /// during the database structure loading process.
        /// <para>
        /// WARNING: Enabling this option may pose security risks. 
        /// </para>
        /// </summary>
        public bool AllowCustomSqlQueries { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether Intellisense is enabled.
        /// When set to true, Intellisense feature will be enabled in the application.
        /// </summary>
        /// <remarks>
        /// WARNING: Enabling Intellisense requires returning class information from the assembly.
        /// </remarks>
        public bool EnableIntelliSense { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of assemblies needed for IntelliSense highlighting.
        /// </summary>
        /// <remarks>
        /// The assemblies listed here will be used to provide class information for IntelliSense highlighting.
        /// </remarks>
        public List<string> IntelliSenseAssemblies { get; set; } = new();

        /// <summary>
        /// Callback that is invoked after a new <see cref="WebReport"/> is created from the Online Designer.
        /// This allows the user to customize or configure the report before it is rendered or processed further.
        /// </summary>
        public Action<WebReport>? OnWebReportCreated { get; set; }
    }
}