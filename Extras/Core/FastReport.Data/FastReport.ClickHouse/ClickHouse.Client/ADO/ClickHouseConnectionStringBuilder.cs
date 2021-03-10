using System;
using System.Data.Common;
using System.Globalization;

namespace ClickHouse.Client.ADO
{
    public class ClickHouseConnectionStringBuilder : DbConnectionStringBuilder
    {
        public ClickHouseConnectionStringBuilder()
        {
        }

        public ClickHouseConnectionStringBuilder(string connectionString) => ConnectionString = connectionString;

        public string Database
        {
            get => TryGetValue("Database", out var value) ? value as string : "default";
            set => this["Database"] = value;
        }

        public string Username
        {
            get => TryGetValue("Username", out var value) ? value as string : "default";
            set => this["Username"] = value;
        }

        public string Password
        {
            get => TryGetValue("Password", out var value) ? value as string : string.Empty;
            set => this["Password"] = value;
        }

        public string Protocol
        {
            get => TryGetValue("Protocol", out var value) ? value as string : "http";
            set => this["Protocol"] = value;
        }

        public string Host
        {
            get => TryGetValue("Host", out var value) ? value as string : "localhost";
            set => this["Host"] = value;
        }

        public bool Compression
        {
            get => TryGetValue("Compression", out var value) ? "true".Equals(value as string, StringComparison.OrdinalIgnoreCase) : true;
            set => this["Compression"] = value;
        }

        public bool UseSession
        {
            get => TryGetValue("UseSession", out var value) ? "true".Equals(value as string, StringComparison.OrdinalIgnoreCase) : false;
            set => this["UseSession"] = value;
        }

        public string SessionId
        {
            get => TryGetValue("SessionId", out var value) ? value as string : null;
            set => this["SessionId"] = value;
        }

        public ushort Port
        {
            get => TryGetValue("Port", out var value) && value is string @string && ushort.TryParse(@string, out var @ushort) ? @ushort : (ushort)8123;
            set => this["Port"] = value;
        }

        [Obsolete]
        public string Driver
        {
            get => TryGetValue("Driver", out var value) ? value as string : null;
            set => this["Driver"] = value;
        }

        public TimeSpan Timeout
        {
            get
            {
                return TryGetValue("Timeout", out var value) && value is string @string && double.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out var timeout)
                    ? TimeSpan.FromSeconds(timeout)
                    : TimeSpan.FromMinutes(2);
            }
            set => this["Timeout"] = value.TotalSeconds;
        }
    }
}
