using System;
using System.Text;

namespace FastReport.Data
{
    /// <summary>
    /// Represents the RavenDBDataConnection connection string builder.
    /// </summary>
    /// <remarks>
    /// Use this class to parse connection string returned by the <b>RavenDBDataConnection</b> class.
    /// </remarks>
    public class RavenDBConnectionStringBuilder
    {
        #region Fields

        private string databaseName;
        private string host;
        private string certificatePath;
        private string password;
        private string connectionString;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string CertificatePath
        {
            get { return certificatePath; }
            set { certificatePath = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenDBConnectionStringBuilder"/> class.
        /// </summary>
        public RavenDBConnectionStringBuilder()
        {
            connectionString = "";
            host = "";
            databaseName = "";
            certificatePath = "";
            password = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenDBConnectionStringBuilder"/> class with a specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string value.</param>
        public RavenDBConnectionStringBuilder(string connectionString)
        {
            this.connectionString = connectionString;
            string[] parts = connectionString.Split(';');
            if (parts[0].Contains("Url"))
            {
                host = parts[0].Replace("Url = ", "");
            }
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Contains("Database"))
                {
                    databaseName = parts[i].Replace("Database=", "");
                }
                if (parts[i].Contains("User"))
                {
                    certificatePath = parts[i].Replace("User=", "");
                }
                if (parts[i].Contains("Password"))
                {
                    password = parts[i].Replace("Password=", "");
                }
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Url = ");
            sb.Append(host);
            if (!String.IsNullOrEmpty(databaseName))
            {
                sb.AppendFormat(";Database={0}", databaseName);
            }
            if (!String.IsNullOrEmpty(certificatePath))
            {
                sb.AppendFormat(";User={0}", certificatePath);
            }
            if (!String.IsNullOrEmpty(password))
            {
                sb.AppendFormat(";Password={0}", password);
            }
            return sb.ToString();
        }

        #endregion Public Methods
    }
}
