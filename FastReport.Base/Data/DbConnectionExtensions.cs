#if NETFRAMEWORK
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Contains async extension methods for DbConnection class.
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Returns schema information for the datasource.
        /// </summary>
        /// <param name="dbConnection">The DbConnection instance.</param>
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        /// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A DataTable that contains schema information.</returns>
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

        /// <summary>
        /// Returns schema information for the datasource.
        /// </summary>
        /// <param name="dbConnection">The DbConnection instance.</param>
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A DataTable that contains schema information.</returns>
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

        /// <summary>
        /// Disposes the connection object.
        /// </summary>
        /// <param name="dbConnection">The DbConnection instance.</param>
        /// <returns>The Task object.</returns>
        public static Task DisposeAsync(this DbConnection dbConnection)
        {
            dbConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}
#endif