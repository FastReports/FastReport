#if NETFRAMEWORK
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    public static class DbConnectionExtensions
    {
        public static Task<DataTable> GetSchemaAsync(this DbConnection dbConnection, string collectionName, string[] restrictionValues,
    CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<DataTable>(cancellationToken);
            }

            try
            {
                return Task.FromResult(dbConnection.GetSchema(collectionName, restrictionValues));
            }
            catch (Exception e)
            {
                return Task.FromException<DataTable>(e);
            }
        }

        public static Task<DataTable> GetSchemaAsync(this DbConnection dbConnection,
            string collectionName,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<DataTable>(cancellationToken);
            }

            try
            {
                return Task.FromResult(dbConnection.GetSchema(collectionName));
            }
            catch (Exception e)
            {
                return Task.FromException<DataTable>(e);
            }
        }

        public static Task DisposeAsync(this DbConnection dbConnection)
        {
            dbConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}
#endif