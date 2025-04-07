using System;
using System.Data;
using System.Data.Common;

namespace FastReport.Data
{
    /// <summary>
    /// <see cref="JsonConnectionType"/> represents a connection to a JSON.
    /// </summary>
    public sealed class JsonConnectionType : DbConnection
    {
        public override string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Database => throw new NotSupportedException("It's just JSON, not database");

        public override string DataSource => throw new NotSupportedException("It's just JSON, not database");

        public override string ServerVersion => throw new NotSupportedException("It's just JSON, server version not supported");

        public override ConnectionState State => throw new NotSupportedException("It's just JSON");

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException("It's just JSON, can't change database");
        }

        public override void Close()
        {
            throw new NotSupportedException("It's just JSON, open/close not supported");
        }

        public override void Open()
        {
            throw new NotSupportedException("It's just JSON, open/close not supported");
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException("It's just JSON, transaction not supported");
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotSupportedException("It's just JSON, command not supported");
        }
    }
}
