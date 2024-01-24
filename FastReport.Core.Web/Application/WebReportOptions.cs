#if !WASM
using FastReport.Web.Cache;
#endif

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FastReport.Web
{
    public sealed class WebReportOptions
    {
#if !WASM
        public CacheOptions CacheOptions { get; set; } = new CacheOptions();

        /// <summary>
        /// SMTP server settings for sending the report by e-mail
        /// </summary>
        public EmailExportOptions EmailExportOptions { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether custom SQL queries can be executed.
        /// When set to true, users will be able to specify custom table names and SQL expressions
        /// during the database structure loading process.
        /// <para>
        /// WARNING: Enabling this option may pose security risks. 
        /// </para>
        /// </summary>
        public bool AllowCustomSqlQueries { get; set; } = false;
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
