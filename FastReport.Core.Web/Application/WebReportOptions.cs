#if !WASM
using FastReport.Web.Cache;
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Http;

namespace FastReport.Web
{
    public sealed class WebReportOptions
    {
#if !WASM
        /// <summary>
        /// Contains settings that define how reports are cached, including storage duration and caching strategy.
        /// </summary>
        public CacheOptions CacheOptions { get; set; } = new CacheOptions();

        /// <summary>
        /// SMTP server settings for sending the report by e-mail
        /// </summary>
        public EmailExportOptions EmailExportOptions { get; set; } = null;

        /// <summary>
        /// Provides options related to the Online Report Designer configuration and behavior.
        /// </summary>
        public DesignerOptions Designer { get; set; } = new DesignerOptions();

        /// <summary>
        /// Gets or sets a value indicating whether custom SQL queries can be executed.
        /// When set to true, users will be able to specify custom table names and SQL expressions
        /// during the database structure loading process.
        /// <para>
        /// WARNING: Enabling this option may pose security risks. 
        /// </para>
        /// </summary>
        [Obsolete("Please, use Designer.AllowCustomSqlQueries")]
        public bool AllowCustomSqlQueries { get => Designer.AllowCustomSqlQueries; set => Designer.AllowCustomSqlQueries = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Intellisense is enabled.
        /// When set to true, Intellisense feature will be enabled in the application.
        /// </summary>
        /// <remarks>
        /// WARNING: Enabling Intellisense requires returning class information from the assembly.
        /// </remarks>
        [Obsolete("Please, use Designer.EnableIntelliSense")]
        public bool EnableOnlineDesignerIntelliSense { get => Designer.EnableIntelliSense; set => Designer.EnableIntelliSense = value; }

        /// <summary>
        /// Gets or sets the list of assemblies needed for IntelliSense highlighting.
        /// </summary>
        /// <remarks>
        /// The assemblies listed here will be used to provide class information for IntelliSense highlighting.
        /// </remarks>
        [Obsolete("Please, use Designer.IntelliSenseAssemblies")]
        public List<string> IntelliSenseAssemblies { get => Designer.IntelliSenseAssemblies; set => Designer.IntelliSenseAssemblies = value; }
#else
        /// <summary>
        /// Used to access .NET libraries to compile the report script
        /// </summary>
        /// <example>
        /// <code>
        /// options.HttpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        /// </code>
        /// </example>
        public HttpClient? HttpClient { get; set; }

#endif

    }
}
