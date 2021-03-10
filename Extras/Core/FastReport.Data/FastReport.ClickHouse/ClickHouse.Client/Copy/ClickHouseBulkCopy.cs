using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Readers;
using ClickHouse.Client.Formats;
using ClickHouse.Client.Properties;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Copy
{
    public class ClickHouseBulkCopy : IDisposable
    {
        private readonly ClickHouseConnection connection;
        private long rowsWritten = 0;

        public ClickHouseBulkCopy(ClickHouseConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Gets or sets size of batch in rows.
        /// </summary>
        public int BatchSize { get; set; } = 100000;

        /// <summary>
        /// Gets or sets maximum number of parallel processing tasks.
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 4;

        /// <summary>
        /// Gets or sets name of destination table to insert to. "SELECT ..columns.. LIMIT 0" query is performed before insertion.
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Gets total number of rows written by this instance.
        /// </summary>
        public long RowsWritten => Interlocked.Read(ref rowsWritten);

        public Task WriteToServerAsync(IDataReader reader) => WriteToServerAsync(reader, CancellationToken.None);

        public Task WriteToServerAsync(IDataReader reader, CancellationToken token)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return WriteToServerAsync(reader.AsEnumerable(), reader.GetColumnNames(), token);
        }

        public Task WriteToServerAsync(DataTable table, CancellationToken token)
        {
            if (table is null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var rows = table.Rows.Cast<DataRow>().Select(r => r.ItemArray); // enumerable
            var columns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            return WriteToServerAsync(rows, columns, token);
        }

        public Task WriteToServerAsync(IEnumerable<object[]> rows) => WriteToServerAsync(rows, null, CancellationToken.None);

        public Task WriteToServerAsync(IEnumerable<object[]> rows, IReadOnlyCollection<string> columns) => WriteToServerAsync(rows, columns, CancellationToken.None);

        public Task WriteToServerAsync(IEnumerable<object[]> rows, CancellationToken token) => WriteToServerAsync(rows, null, token);

        public async Task WriteToServerAsync(IEnumerable<object[]> rows, IReadOnlyCollection<string> columns, CancellationToken token)
        {
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (string.IsNullOrWhiteSpace(DestinationTableName))
            {
                throw new InvalidOperationException(Resources.DestinationTableNotSetMessage);
            }

            ClickHouseType[] columnTypes = null;
            string[] columnNames = columns?.ToArray();

            using (var reader = (ClickHouseDataReader)await connection.ExecuteReaderAsync($"SELECT {GetColumnsExpression(columns)} FROM {DestinationTableName} LIMIT 0"))
            {
                columnTypes = reader.GetClickHouseColumnTypes();
                columnNames ??= reader.GetColumnNames();
            }
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = columnNames[i].EncloseColumnName();

            var tasks = new Task[MaxDegreeOfParallelism];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.CompletedTask;
            }

            foreach (var batch in rows.Batch(BatchSize))
            {
                token.ThrowIfCancellationRequested();
                while (true)
                {
                    var completedTaskIndex = Array.FindIndex(tasks, t => t.IsCompleted);
                    if (completedTaskIndex >= 0)
                    {
                        // propagate exception if one happens
                        // 'await' instead of 'Wait()' to avoid dealing with AggregateException
                        await tasks[completedTaskIndex].ConfigureAwait(false);
                        var task = PushBatch(batch, columnTypes, columnNames, token);
                        tasks[completedTaskIndex] = task;
                        break; // while (true); go to next batch
                    }
                    else
                    {
                        await Task.WhenAny(tasks).ConfigureAwait(false);
                    }
                }
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() => connection?.Dispose();

        private string GetColumnsExpression(IReadOnlyCollection<string> columns) => columns == null || columns.Count == 0 ? "*" : string.Join(",", columns);

        private async Task PushBatch(ICollection<object[]> rows, ClickHouseType[] columnTypes, string[] columnNames, CancellationToken token)
        {
            var query = $"INSERT INTO {DestinationTableName} ({string.Join(", ", columnNames)}) FORMAT RowBinary";
            bool useInlineQuery = await connection.SupportsInlineQuery();

            using var stream = new MemoryStream() { Capacity = 512 * 1024 };
            using (var gzipStream = new BufferedStream(new GZipStream(stream, CompressionLevel.Fastest, true), 256 * 1024))
            {
                if (useInlineQuery)
                {
                    using var textWriter = new StreamWriter(gzipStream, Encoding.UTF8, 4 * 1024, true);
                    textWriter.WriteLine(query);
                    query = null; // Query was already written to POST body
                }

                using var writer = new ExtendedBinaryWriter(gzipStream);
                using var streamer = new BinaryStreamWriter(writer);
                foreach (var row in rows)
                {
                    for (var i = 0; i < row.Length; i++)
                    {
                        streamer.Write(columnTypes[i], row[i]);
                    }
                }
            }
            stream.Seek(0, SeekOrigin.Begin);

            await connection.PostStreamAsync(query, stream, true, token).ConfigureAwait(false);
            Interlocked.Add(ref rowsWritten, rows.Count);
        }
    }
}
