namespace FastReport.Web
{
    /// <summary>
    /// SMTP server options for sending the report by mail
    /// </summary>
    public sealed class EmailExportOptions
    {
        /// <summary>
        /// Determines whether to enable the SSL protocol.
        /// </summary>
        /// <remarks>
        /// The default value for this property is <b>false</b>.
        /// </remarks>
        public bool EnableSSL { get; set; }

        /// <summary>
        /// SMTP account username.
        /// </summary>
        /// <remarks>
        /// Specify the <see cref="Username"/> and <see cref="Password"/> properties if your host requires authentication.
        /// </remarks>
        public string Username { get; set; }

        /// <summary>
        /// Template that will be used to create a new message.
        /// </summary>
        public string MessageTemplate { get; set; }

        /// <summary>
        /// The name that will appear next to the address from which the e-mail is sent.
        /// </summary>
        /// <remarks>
        /// This property contains your name (for example, "John Smith").
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// SMTP host name or IP address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The address from which the e-mail will be sent
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// SMTP server password.
        /// </summary>
        /// <remarks>
        /// Specify the <see cref="Username"/> and <see cref="Password"/> properties if your host requires
        /// authentication.
        /// </remarks>
        public string Password { get; set; }

        /// <summary>
        /// SMTP port.
        /// </summary>
        /// <remarks>
        /// The default value for this property is <b>25</b>.
        /// </remarks>
        public int Port { get; set; } = 25;
    }

    public sealed class EmailExportParams
    {
        public string ExportFormat { get; set; }

        public string MessageBody { get; set; }

        public string NameAttachmentFile { get; set; }

        public string Subject { get; set; }

        public string Address { get; set; }
    }
}